#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class HellbatSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Hellbat;
		public override string soulDescription => "Fire a spread of flaming bats.";

		public override short cooldown => 20;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(2 + (.5 * stack));
		public override bool SoulUpdate(Player p, short stack)
		{
			int minAmount = 2;
			int maxAmount = 5;
			int damage = 7 + stack;

			if (stack >= 5)
			{
				minAmount++;
				maxAmount++;
			}
			if (stack >= 9)
			{
				minAmount++;
				maxAmount++;
			}

			int amount = Main.rand.Next(minAmount, maxAmount);

			for (int i = 0; i < amount; ++i)
			{
				Vector2 pos = p.position + new Vector2(Main.rand.Next(p.width + 1), Main.rand.Next(p.height/2 + 1));
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - pos) * 6;
				Projectile.NewProjectile(pos, velocity.RotatedByRandom(.2), ModContent.ProjectileType<HellbatSoulProj>(), damage, 0, p.whoAmI);
			}

			return (true);
		}
	}

	public class HellbatSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flaming Bat");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 22;

			projectile.penetrate = 1;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ignoreWater = false;

			if (Main.rand != null)
				projectile.scale = Main.rand.Next(4, 9) * .1f;
		}

		public override bool PreAI()
		{
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
			if (projectile.direction == -1)
				projectile.rotation += (float)Math.PI;
			projectile.spriteDirection = -projectile.direction;

			if (projectile.frameCounter++ >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			Lighting.AddLight(projectile.Center, new Vector3(.55f, .1f, .3f));

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.OnFire, 180);
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath4, projectile.position);
			for (int i = 0; i < 10; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)];
				d.velocity *= 2f;
			}
		}
	}
}
