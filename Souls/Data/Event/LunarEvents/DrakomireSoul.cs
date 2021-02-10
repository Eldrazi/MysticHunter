#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class DrakomireSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SolarDrakomire;
		public override string soulDescription => "Summon a charging Drakomire.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 210 + 20 * stack;

			Vector2 velocity = new Vector2(8 * p.direction, 0);
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<DrakomireSoul_Proj>(), damage, 1, p.whoAmI);

			return (true);
		}
	}

	public class DrakomireSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.SolarDrakomire;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Drakomire");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.SolarDrakomire];
		}
		public override void SetDefaults()
		{
			projectile.width = 46;
			projectile.height = 34;

			projectile.penetrate = -1;

			projectile.friendly = true;

			this.drawOriginOffsetY = 6;
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
					if (projectile.frameCounter++ >= 4)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % 8;
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
				Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0f, 0f, 100);
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
