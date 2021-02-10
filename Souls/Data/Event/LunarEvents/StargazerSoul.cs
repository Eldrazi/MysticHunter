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
	public class StargazerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.StardustSoldier;
		public override string soulDescription => "Fire a thin blue laser.";

		public override short cooldown => 30;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<StargazerSoul_Proj>())
				{
					Main.projectile[i].timeLeft = 60;
					Main.projectile[i].netUpdate = true;

					return (true);
				}
			}

			int damage = 210 + 10 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center);
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<StargazerSoul_Proj>(), damage, 0, p.whoAmI);

			return (true);
		}
	}

	public class StargazerSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.StardustSoldierLaser;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardust Laser");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.timeLeft = 60;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
			{
				projectile.velocity = -Vector2.UnitY;
			}

			projectile.position = owner.Center - projectile.Size / 2f + new Vector2(0, -owner.gfxOffY);

			projectile.scale = 0.5f;

			projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
			
			float pointAmount = 2f;
			Vector2 samplingPoint = projectile.Center;

			float[] array = new float[(int)pointAmount];
			Collision.LaserScan(samplingPoint, projectile.velocity, 0, 2400f, array);

			float length = 0;
			for (int i = 0; i < array.Length; ++i)
			{
				length += array[i];
			}

			length /= pointAmount;
			projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], length, 0.5f);

			return (false);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float collisionPoint = 0f;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center,
				projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, ref collisionPoint))
			{
				return (true);
			}
			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.velocity == Vector2.Zero)
			{
				return (false);
			}

			Texture2D texture2D17 = Main.projectileTexture[projectile.type];
			float num203 = projectile.localAI[1];
			Color color38 = new Color(255, 255, 255, 0) * 0.9f;
			Rectangle rectangle16 = new Rectangle(0, 0, texture2D17.Width, 22);
			Vector2 value25 = new Vector2(0f, Main.player[projectile.owner].gfxOffY);
			spriteBatch.Draw(texture2D17, projectile.Center.Floor() - Main.screenPosition + value25, rectangle16, color38, projectile.rotation, rectangle16.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			num203 -= 33f * projectile.scale;
			Vector2 value26 = projectile.Center.Floor();
			value26 += projectile.velocity * projectile.scale * 10.5f;
			rectangle16 = new Rectangle(0, 25, texture2D17.Width, 28);
			if (num203 > 0f)
			{
				float num204 = 0f;
				while (num204 + 1f < num203)
				{
					if (num203 - num204 < rectangle16.Height)
					{
						rectangle16.Height = (int)(num203 - num204);
					}
					spriteBatch.Draw(texture2D17, value26 - Main.screenPosition + value25, rectangle16, color38, projectile.rotation, new Vector2(rectangle16.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
					num204 += rectangle16.Height * projectile.scale;
					value26 += projectile.velocity * rectangle16.Height * projectile.scale;
				}
			}
			rectangle16 = new Rectangle(0, 56, texture2D17.Width, 22);
			spriteBatch.Draw(texture2D17, value26 - Main.screenPosition + value25, rectangle16, color38, projectile.rotation, texture2D17.Frame().Top(), projectile.scale, SpriteEffects.None, 0f);
			return (false);
		}
	}
}
