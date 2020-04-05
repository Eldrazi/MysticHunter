using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class FaceMonsterSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.FaceMonster;
		public string soulDescription => "Summons a charging face monster.";

		public short cooldown => 180;

		public SoulType soulType => SoulType.Red;
		
		public short ManaCost(Player p, short stack) => (short)(5 + 3 * stack);
		public bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<FaceMonsterSoulProj>(), 20 + stack, .1f, p.whoAmI);
			return (true);
		}
	}

	public class FaceMonsterSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_181";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Face Monster");
			Main.projFrames[projectile.type] = 16;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.penetrate = 3;

			projectile.melee = true;
			projectile.friendly = true;

			drawOriginOffsetY = -28;
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
					projectile.frame = 14;
					projectile.frameCounter = 0;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					if (projectile.frameCounter++ >= 2)
					{
						projectile.frame--;
						if (projectile.frame < 0)
							projectile.frame = 13;
						projectile.frameCounter = 0;
					}
					if (projectile.frame == 14)
						projectile.frame = 13;
				}
			}
			else
			{
				projectile.frame = 14;
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
