using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class AnomuraFungusSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.AnomuraFungus;
		public override string soulDescription => "Summons a heavy anomura fungus.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<AnomuraFungusSoulProj>(), 10 + 2 * stack, 1.5f + .2f * stack, p.whoAmI);
			return (true);
		}
	}

	public class AnomuraFungusSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_257";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Anomura Fungus");
			Main.projFrames[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = 42;
			projectile.height = 32;

			projectile.penetrate = 5;

			projectile.melee = true;
			projectile.friendly = true;

			drawOriginOffsetY = -6;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0] == 0)
			{
				// Set the correct direction of the projectile.
				if (projectile.velocity.X > 0)
					projectile.spriteDirection = -1;
				else
					projectile.spriteDirection = 1;

				// Animate the projectile.
				if (projectile.velocity.Y != 0)
				{
					projectile.frame = 4;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					if (projectile.frameCounter++ >= 2)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
					}

					if (projectile.frame == 4)
						projectile.frame = 0;
				}
			}
			else
			{
				projectile.frame = 4;
				projectile.rotation -= projectile.spriteDirection * .03f;
			}

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.ai[0] = 1;
				projectile.timeLeft = 30;
				projectile.netUpdate = true;
				projectile.tileCollide = false;

				projectile.velocity.Y = -2;
				projectile.velocity.X = oldVelocity.X * .5f;
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
