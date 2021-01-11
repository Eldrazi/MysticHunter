#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class WerewolfSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Werewolf;
		public override string soulDescription => "Claw at your enemies.";

		public override short cooldown => 16;

		public override SoulType soulType => SoulType.Red;

		int direction = -1;
		public override short ManaCost(Player p, short stack) => 1;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 90;
			if (stack >= 5)
				damage += 20;
			if (stack >= 9)
				damage += 20;

			Vector2 spawnPos = p.MountedCenter + new Vector2(34 * p.direction, 0);

			Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<WerewolfSoulProj>(), damage, 0, p.whoAmI, direction);

			direction *= -1;
			return (true);
		}
	}

	public class WerewolfSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Claw");
			Main.projFrames[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 28;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.manualDirectionChange = true;

			drawOffsetX = -10;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (!owner.active || owner.dead)
				projectile.Kill();

			if (projectile.frameCounter++ >= 3)
			{
				projectile.frameCounter = 0;

				if (projectile.frame++ >= Main.projFrames[projectile.type])
					projectile.Kill();
			}

			projectile.direction = projectile.spriteDirection = (int)projectile.ai[0];

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!target.boss && Main.rand.Next(6) == 0)
				target.AddBuff(BuffID.Bleeding, 120);
		}

		public override bool? CanDamage()
			=> projectile.frame == 2;
	}
}
