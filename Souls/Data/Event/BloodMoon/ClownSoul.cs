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
	internal sealed class ClownSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Clown;
		public override string soulDescription => "Lob a smiling bomb.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 50 + 5 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8f;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<ClownSoulProj>(), damage, 8f, p.whoAmI);

			return (true);
		}
	}

	internal sealed class ClownSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.HappyBomb;

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;
			
			projectile.timeLeft = 300;
			projectile.penetrate = -1;
			
			projectile.friendly = true;
			
			drawOriginOffsetY = 4;
		}

		public override bool PreAI()
		{
			// Explosion.
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.ai[1] = 0f;
				projectile.alpha = 255;
				projectile.tileCollide = false;

				projectile.position = projectile.Center;
				projectile.width = projectile.height = 128;
				projectile.Center = projectile.position;
			}
			// Dust logic.
			else
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100)];
				newDust.scale = 0.1f + Main.rand.Next(5) * 0.1f;
				newDust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
				newDust.noGravity = true;
				newDust.position = projectile.Center + new Vector2(0f, -projectile.height / 2).RotatedBy(projectile.rotation) * 1.1f;

				newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100)];
				newDust.scale = 1f + Main.rand.Next(5) * 0.1f;
				newDust.noGravity = true;
				newDust.position = projectile.Center + new Vector2(0f, -projectile.height / 2 - 6).RotatedBy(projectile.rotation) * 1.1f;
			}

			if (++projectile.ai[0] > 5)
			{
				if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
				{
					projectile.velocity.X *= 0.97f;
					if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
					{
						projectile.velocity.X = 0f;
						projectile.netUpdate = true;
					}
				}

				projectile.ai[0] = 10f;
				projectile.velocity.Y += 0.2f;
			}

			projectile.rotation += projectile.velocity.X * 0.1f;

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.timeLeft > 3)
			{
				projectile.timeLeft = 3;
			}
		}
		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			if (projectile.timeLeft > 3)
			{
				projectile.timeLeft = 3;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = oldVelocity.X * -0.4f;
			}
			if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
			{
				projectile.velocity.Y = oldVelocity.Y * -0.4f;
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item14, projectile.position);
			
			projectile.position = projectile.Center;
			projectile.width = projectile.height = 22;
			projectile.Center = projectile.position;

			for (int i = 0; i < 20; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 1.4f;
			}

			for (int i = 0; i < 10; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2.5f)];
				newDust.velocity *= 5f;
				newDust.noGravity = true;

				newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 3f;
			}

			Gore newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
			newGore.velocity *= 0.4f;
			newGore.velocity += Vector2.One;

			newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
			newGore.velocity *= 0.4f;
			newGore.velocity.X -= 1f;
			newGore.velocity.Y += 1f;

			newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
			newGore.velocity *= 0.4f;
			newGore.velocity.X += 1f;
			newGore.velocity.Y -= 1f;

			newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
			newGore.velocity *= 0.4f;
			newGore.velocity -= Vector2.One;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Vector2 origin = texture.Size() / 2 + new Vector2(drawOriginOffsetX, drawOriginOffsetY);

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
