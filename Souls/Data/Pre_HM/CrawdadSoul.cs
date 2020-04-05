using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class CrawdadSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Crawdad;
		public string soulDescription => "Claw at your enemies.";

		public short cooldown => 300;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => 15;
		public bool SoulUpdate(Player p, short stack)
		{
			int damage = 20;
			int clawState = 0;
			if (stack >= 5)
			{
				clawState++;
				damage += 15;
			}
			if (stack >= 9)
			{
				damage += 15;
				clawState++;
			}

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<CrawdadSoulProj>(), damage, .2f + .1f * stack, p.whoAmI, clawState);
			return (true);
		}
	}
	
	public class CrawdadSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Claw");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.melee = true;
			projectile.friendly = true;
			projectile.ownerHitCheck = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (!owner.active || owner.dead)
				projectile.Kill();

			projectile.alpha -= 20;
			if (projectile.alpha < 0)
				projectile.alpha = 0;

			if (projectile.ai[0] == 0) // Small claw.
				projectile.scale = .8f;
			else if (projectile.ai[0] == 1) // Medium claw.
				projectile.scale = 1f;
			else if (projectile.ai[0] == 2) // Large claw.
				projectile.scale = 1.2f;

			projectile.ai[1]++;
			projectile.position = owner.position + new Vector2(projectile.ai[1] * 3 * owner.direction, 8);

			// Animate projectile.
			if (projectile.frameCounter++ >= 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;

				if (projectile.frame >= Main.projFrames[projectile.type])
					projectile.Kill();
			}

			// Set correct projectile direction.
			projectile.spriteDirection = -owner.direction;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100, Color.DarkRed, projectile.scale - .2f);
		}
	}
}
