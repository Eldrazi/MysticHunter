using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class IceBatSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.IceBat;
		public string soulName => "Ice Bat";
		public string soulDescription => "Fire a spread of icy bats.";

		public short cooldown => 20;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => (short)(2 + (.5 * stack));
		public bool SoulUpdate(Player p, short stack)
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
				Projectile.NewProjectile(pos, velocity.RotatedByRandom(.2), ProjectileType<IceBatSoulProj>(), damage, 0, p.whoAmI);
			}

			return (true);
		}
	}

	public class IceBatSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icy Bat");
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

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.Frostburn, 180);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.NPCDeath4, projectile.position);
			for (int i = 0; i < 10; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)];
				d.velocity *= 2f;
			}
		}
	}
}
