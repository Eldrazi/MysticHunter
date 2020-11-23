using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class AntlionChargerSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.WalkingAntlion;
		public override string soulDescription => "Summons an aggressive antlion charger.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<AntlionChargerSoulProj>(), 20 + 3 * stack, .5f, p.whoAmI);
			return (true);
		}
	}

	public class AntlionChargerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_508";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Antlion Charger");
			Main.projFrames[projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			projectile.width = 33;
			projectile.height = 32;

			projectile.penetrate = 3;

			projectile.melee = true;
			projectile.friendly = true;

			drawOriginOffsetY = -4;
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
					projectile.frame = 1;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					if (projectile.frameCounter++ >= 2)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
					}
					if (projectile.frame < 2)
						projectile.frame = 2;
				}
			}
			else
			{
				projectile.frame = 1;
				projectile.rotation -= projectile.spriteDirection * .03f;
			}

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = true;
			return (true);
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
