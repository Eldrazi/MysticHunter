#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.PumpkinMoon
{
	public class HeadlessHorsemanSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.HeadlessHorseman;
		public override string soulDescription => "Summon a charging Headless Horseman.";

		public override short cooldown => 120;//1200;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 80;

			if (stack >= 5)
			{
				damage += 20;
			}
			if (stack >= 9)
			{
				damage += 20;
			}

			Vector2 velocity = new Vector2(8 * p.direction, 0);
			Projectile.NewProjectile(p.Center + new Vector2(0, -22), velocity, ModContent.ProjectileType<HeadlessHorsemanSoulProj>(), damage, 1, p.whoAmI);

			return (true);
		}
	}

	public class HeadlessHorsemanSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.HeadlessHorseman;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 11;
			DisplayName.SetDefault("Headless Horseman");
		}
		public override void SetDefaults()
		{
			projectile.width = 60;
			projectile.height = 56;

			projectile.friendly = true;

			projectile.penetrate = 3;

			drawOffsetX = -34;
			drawOriginOffsetY = -30;
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
				projectile.frame = 0;
				projectile.rotation -= projectile.spriteDirection * .03f;
			}
			
			if (projectile.ai[1]++ >= 120)
			{
				projectile.ai[1] = 0;
				if (Main.myPlayer == projectile.owner)
				{
					Vector2 velocity = new Vector2(projectile.velocity.X * .5f, -Main.rand.NextFloat());
					Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.JackOLantern, projectile.damage, projectile.knockBack, projectile.owner);
				}
			}

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
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
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.FlameBurst, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
