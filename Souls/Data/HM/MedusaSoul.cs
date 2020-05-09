using System;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class MedusaSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Medusa;
		public override string soulDescription => "Fire a petrifying beam.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 1;
		public override bool SoulUpdate(Player p, short stack)
		{
			bool spawn = true;

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<MedusaSoulProj>() && Main.projectile[i].owner == p.whoAmI)
				{
					spawn = false;
					break;
				}
			}

			if (spawn)
				Projectile.NewProjectile(p.Center, Main.MouseWorld - p.Center, ProjectileType<MedusaSoulProj>(), 20 + 2 * stack, 0, p.whoAmI);
			return (true);
		}
	}

	public class MedusaSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_461";

		private const float MaxRange = 600;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Petrifying Beam");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			Vector2 direction = projectile.velocity;

			if (Main.myPlayer == owner.whoAmI)
			{
				if (owner.dead || owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Red].soulNPC != NPCID.Medusa ||
					!MysticHunter.Instance.RedSoulActive.Current)
				{
					projectile.Kill();
					return (false);
				}

				direction = Vector2.Normalize(Main.MouseWorld - owner.Center);
			}

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
				projectile.velocity = -Vector2.UnitY;

			projectile.position = owner.RotatedRelativePoint(owner.MountedCenter) + direction * 16f - projectile.Size / 2f;
			projectile.velocity = direction;

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
				projectile.velocity = -Vector2.UnitY;

			float rotation = projectile.velocity.ToRotation();
			projectile.rotation = rotation - MathHelper.PiOver2;

			float[] laserScanSamples = new float[2];
			Collision.LaserScan(projectile.Center, projectile.velocity, 0, MaxRange, laserScanSamples);

			float endPointLength = laserScanSamples.Sum() / laserScanSamples.Length;
			projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], endPointLength, .5f);

			// Visual effects.
			Vector2 endPoint = projectile.Center + projectile.velocity * (projectile.localAI[1] - 8);
			for (int i = 0; i < 2; i++)
			{
				float rot = projectile.rotation + ((Main.rand.Next(2) == 1) ? -1 : 1) * MathHelper.PiOver2;
				float randRot = Main.rand.NextFloat() * .8f + 1;

				Vector2 dustVelocity = new Vector2((float)Math.Cos(rot) * randRot, (float)Math.Sin(rot) * randRot);
				Dust d = Main.dust[Dust.NewDust(endPoint, 0, 0, 226, dustVelocity.X, dustVelocity.Y, 0, (Color)GetAlpha(Color.White), 1.2f)];
				d.noGravity = true;
			}

			DelegateMethods.v3_1 = new Vector3(0.4f, 0.85f, 0.9f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CastLight);

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!target.boss && Main.rand.Next(4) == 0)
				target.AddBuff(BuffID.Stoned, 120);
		}

		public override bool? CanCutTiles()
			=> true;
		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
				projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22 * projectile.scale, ref collisionPoint);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return (new Color(160, 240, 120));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.velocity == Vector2.Zero)
				return (false);

			Texture2D tex = Main.projectileTexture[projectile.type];
			float length = projectile.localAI[1];

			Color drawColor = (Color)GetAlpha(lightColor) * 0.9f;
			Rectangle rectangle = new Rectangle(0, 0, tex.Width, 22);

			spriteBatch.Draw(tex, projectile.Center.Floor() - Main.screenPosition, rectangle, drawColor, projectile.rotation, rectangle.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

			length -= 33 * projectile.scale;
			Vector2 center = projectile.Center.Floor();
			center += projectile.velocity * projectile.scale * 10.5f;
			rectangle = new Rectangle(0, 25, tex.Width, 28);
			if (length > 0)
			{
				float currentLength = 0;
				while (currentLength + 1 < length)
				{
					if (length - currentLength < rectangle.Height)
						rectangle.Height = (int)(length - currentLength);

					spriteBatch.Draw(tex, center - Main.screenPosition, rectangle, drawColor, projectile.rotation, new Vector2(rectangle.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);
					currentLength += rectangle.Height * projectile.scale;
					center += projectile.velocity * rectangle.Height * projectile.scale;
				}
			}

			rectangle = new Rectangle(0, 56, tex.Width, 22);
			Main.spriteBatch.Draw(tex, center - Main.screenPosition, rectangle, drawColor, projectile.rotation, tex.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
