#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using Terraria.Audio;
using Terraria.GameContent;

#endregion

namespace MysticHunter.Souls.Data.Event.Sandstorm
{
	public class SandElementalSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SandElemental;
		public override string soulDescription => "Summon a sandnado.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			int damage = 50;

			if (stack >= 5)
			{
				amount++;
				damage += 10;
			}
			if (stack >= 9)
			{
				amount++;
				damage += 10;
			}

			int projectilePadding = 120;
			float xPosStart = Main.MouseWorld.X - (projectilePadding * (amount - 1) * .5f);
			for (int i = 0; i < amount; ++i)
			{
				float xPos = xPosStart + projectilePadding * i;
				Projectile.NewProjectile(xPos, Main.MouseWorld.Y, 0, 0, ModContent.ProjectileType<SandElementalSoulProj>(), damage, .5f, p.whoAmI);
			}
			return (true);
		}
	}

	internal sealed class SandElementalSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SandnadoFriendly;

		private readonly int maxExpandUp = 5, maxExpandDown = 5;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandnado");
		}
		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;

			projectile.scale = .5f;
			projectile.timeLeft = 480;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreAI()
		{
			float timeAlive = 480;

			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = -1;
				SoundEngine.PlaySound(SoundID.Item82, projectile.Center);
			}

			projectile.ai[0]++;
			if (projectile.ai[0] >= timeAlive)
			{
				projectile.Kill();
			}

			if (projectile.localAI[0] >= 30)
			{
				projectile.damage = 0;
				if (projectile.ai[0] < timeAlive - 120)
				{
					projectile.ai[0] = timeAlive - 120 + (projectile.ai[0] % 60);
					projectile.netUpdate = true;
				}
			}
			
			Point tileCoords = projectile.Center.ToTileCoordinates();
			Collision.ExpandVertically(tileCoords.X, tileCoords.Y, out int topY, out int bottomY, maxExpandUp, maxExpandDown);
			topY += 1;
			bottomY -= 1;

			Vector2 topPos = new Vector2(tileCoords.X, topY) * 16 + new Vector2(8);
			Vector2 botPos = new Vector2(tileCoords.X, bottomY) * 16 + new Vector2(8);
			Vector2 midPos = Vector2.Lerp(topPos, botPos, .5f);

			Vector2 size = new Vector2(0, botPos.Y - topPos.Y);
			size.X = size.Y * .2f;

			projectile.width = (int)(size.X * .65f);
			projectile.height = (int)size.Y;
			projectile.Center = midPos;			

			// Check to see if the projectile is still within a viewpoint of the player.
			// If not, slow kill the projectile.
			if (projectile.owner == Main.myPlayer)
			{
				bool keepAlive = false;
				Vector2 ownerTop = Main.player[projectile.owner].Top;
				Vector2 ownerCenter = Main.player[projectile.owner].Center;

				for (float i = 0; i < 1f; i += .05f)
				{
					Vector2 hitPos = Vector2.Lerp(topPos, botPos, i);
					if (Collision.CanHitLine(hitPos, 0, 0, ownerCenter, 0, 0) || Collision.CanHitLine(hitPos, 0, 0, ownerTop, 0, 0))
					{
						keepAlive = true;
						break;
					}
				}

				if (!keepAlive && projectile.ai[0] < timeAlive - 120)
				{
					projectile.ai[0] = timeAlive - 120 + (projectile.ai[0] % 60);
					projectile.netUpdate = true;
				}
			}
			if (projectile.ai[0] < timeAlive - 120)
			{
				float to = .9f;
				float from = -.5f;
				float amount = Main.rand.NextFloat();

				// Get a random dust position within projectile boundaries.
				Vector2 randDustPos = new Vector2(MathHelper.Lerp(.1f, 1, Main.rand.NextFloat()), MathHelper.Lerp(from, to, amount));
				randDustPos.X *= -MathHelper.Lerp(2.2f, .6f, amount);

				Vector2 dustOffset = new Vector2(6, 10) * projectile.scale;
				Vector2 dustSpawnPos = midPos + size * randDustPos * .5f + dustOffset;
				Dust d = Main.dust[Dust.NewDust(dustSpawnPos, 0, 0, DustID.Sandnado)];
				d.position = dustSpawnPos;
				d.customData = midPos + dustOffset;
				d.fadeIn = 1f;
				d.scale = .3f;

				if (randDustPos.X > -1.2f)
				{
					d.velocity.X = 1 + Main.rand.NextFloat();
				}
				d.velocity.Y = Main.rand.NextFloat() * -.5f - 1;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.localAI[0] += 1f;
			target.immune[projectile.owner] = 0;
			projectile.localNPCImmunity[target.whoAmI] = 8;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			float timeAlive = 480;

			float scale = MathHelper.Clamp(projectile.ai[0] / 30, 0, 1);
			if (projectile.ai[0] > timeAlive - 60)
			{
				scale = MathHelper.Lerp(1, 0, (projectile.ai[0] - (timeAlive - 60)) / 60f);
			}

			Point tileCoords = projectile.Center.ToTileCoordinates();
			Collision.ExpandVertically(tileCoords.X, tileCoords.Y, out int topY, out int bottomY, maxExpandUp, maxExpandDown);
			topY += 1;
			bottomY -= 1;

			Vector2 topPos = new Vector2(tileCoords.X, topY) * 16 + new Vector2(8);
			Vector2 botPos = new Vector2(tileCoords.X, bottomY) * 16 + new Vector2(8);

			Vector2 size = new Vector2(0, botPos.Y - topPos.Y);
			size.X = size.Y * .2f;

			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle frame = texture.Frame(1, 1, 0, 0);
			Vector2 origin = frame.Size() / 2f;
			float rotation = -0.06283186f * projectile.ai[0];
			Color color = new Color(212, 192, 100);

			double radians = (projectile.ai[0] * .1f);
			Vector2 spinningPoint = Vector2.UnitY.RotatedBy(radians);

			float currentY = 0;
			float stepSize = 5.1f;
			for (float y = (int)botPos.Y; y > (int)topPos.Y; y -= stepSize)
			{
				currentY += stepSize;

				float heightMod = currentY / size.Y;
				float rotationMod = currentY * MathHelper.TwoPi / -20f;
				float scaleMod = heightMod - .15f;

				Color c = Color.Lerp(Color.Transparent, color, heightMod * 2);
				if (heightMod > .5f)
				{
					c = Color.Lerp(Color.Transparent, color, 2 - heightMod * 2);
				}
				c.A = (byte)(c.A * .5f);
				c *= scale;

				spriteBatch.Draw(
					texture, new Vector2(botPos.X, y) - Main.screenPosition,
					null, c, rotation + rotationMod, origin, projectile.scale + scaleMod, SpriteEffects.None, 0);
			}

			return (false);
		}
	}
}
