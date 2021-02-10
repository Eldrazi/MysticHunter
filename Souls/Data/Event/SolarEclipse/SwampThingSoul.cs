#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SwampThingSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SwampThing;
		public override string soulDescription => "Summons a jungle-spreading Swamp Thing.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 80 + 5 * stack;

			Vector2 velocity = new Vector2(4 * p.direction, 0);
				
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<SwampThingSoul_Proj>(), damage, .5f, p.whoAmI);
			return (true);
		}
	}

	public class SwampThingSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.SwampThing;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Swamp Thing");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.SwampThing];
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 32;

			projectile.melee = true;
			projectile.friendly = true;

			projectile.penetrate = 3;

			drawOriginOffsetY = 6;
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

					if (++projectile.frameCounter >= 3)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
					}

					if (projectile.frame < 7)
						projectile.frame = 7;
				}
			}
			else
			{
				projectile.frame = 6;
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

			// Spread jungle on mud blocks that are being stood/walked on.
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				int tileX = (int)projectile.Center.X / 16;
				int tileY = (int)(projectile.position.Y + projectile.height) / 16;

				if (WorldGen.InWorld(tileX, tileY, 3))
				{
					Tile t = Framing.GetTileSafely(tileX, tileY);
					if (t.active() && t.type == TileID.Mud)
					{
						t.type = TileID.JungleGrass;
						WorldGen.TileFrame(tileX, tileY);

						WorldGen.KillTile(tileX, tileY, true, true, true);
					}
				}
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			this.DrawAroundOrigin(spriteBatch, lightColor);
			return (false);
		}
	}
}
