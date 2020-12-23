using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class MartianWalkerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MartianWalker;
		public override string soulDescription => "Summon a slow Martian Walker.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60 + 5 * stack;

			Vector2 velocity = new Vector2(p.direction, 0);
			Projectile.NewProjectile(p.Center + new Vector2(0, -16), velocity, ProjectileType<MartianWalkerSoulProj>(), damage, 1, p.whoAmI);

			return (true);
		}
	}

	public class MartianWalkerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.MartianWalker;

		float[] velocityValues = { 1f, .8f, .4f, .8f, 1f, .8f, .4f, .8f };

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Martian Walker");
			Main.projFrames[projectile.type] = 8;
		}
		public override void SetDefaults()
		{
			projectile.width = 46;
			projectile.height = 64;

			projectile.melee = true;
			projectile.friendly = true;

			projectile.penetrate = -1;

			drawOffsetX = -26;
			drawOriginOffsetY = -46;
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
					projectile.frame = 0;
				}
				else
				{
					if (projectile.frameCounter++ >= 10)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
					}
					projectile.velocity.X = projectile.direction * velocityValues[projectile.frame];
				}
			}
			else
			{
				projectile.frame = 0;
				projectile.rotation -= projectile.spriteDirection * .03f;
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
