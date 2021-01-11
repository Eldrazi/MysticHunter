#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class FaceMonsterSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.FaceMonster;
		public override string soulDescription => "Summons a charging face monster.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;
		
		public override short ManaCost(Player p, short stack) => (short)(5 + 3 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<FaceMonsterSoulProj>(), 20 + stack, .1f, p.whoAmI);
			return (true);
		}
	}

	public class FaceMonsterSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.FaceMonster;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Face Monster");
			Main.projFrames[projectile.type] = 16;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.penetrate = 3;

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
