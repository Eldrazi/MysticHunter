#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.BloodMoon
{
	public class CorruptGoldfishSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.CorruptGoldfish;
		public override string soulDescription => "Throw a flopping Corrupt Goldfish";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 10 + 2 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<CorruptGoldfishSoulProj>(), damage, 0.5f, p.whoAmI);

			return (true);
		}
	}

	public class CorruptGoldfishSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.CorruptGoldfish;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrupt Goldfish");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.CorruptGoldfish];
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 14;

			projectile.penetrate = -1;
			projectile.timeLeft = 420;

			projectile.friendly = true;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				projectile.spriteDirection = projectile.direction;
			}

			if (projectile.ai[0] == 0)
			{
				projectile.rotation += projectile.velocity.X * .1f * projectile.direction;

				projectile.velocity.Y += 0.2f;
				projectile.velocity.X *= 0.99f;
			}
			else
			{
				projectile.rotation = 0;

				projectile.velocity.Y += 0.2f;
				projectile.velocity.X *= 0.98f;

				if (projectile.frameCounter++ >= 5)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				}

				if (projectile.frame < 4)
				{
					projectile.frame = 4;
				}	
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = projectile.oldVelocity.X * 0.6f;
			}

			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.ai[0] = 1;
				projectile.velocity.Y = -oldVelocity.Y * 0.6f;

				if (oldVelocity.Y > 0 && projectile.velocity.Y > -0.5f)
				{
					projectile.velocity.Y = -3;
					projectile.velocity.X = Main.rand.Next(-3, 4);
					projectile.netUpdate = true;
				}
			}

			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (projectile.tileCollide);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, projectile.velocity.X * .2f, projectile.velocity.Y * .2f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
			Vector2 origin = frame.Size() / 2;
			SpriteEffects effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effects, 0);

			return (false);
		}
	}
}
