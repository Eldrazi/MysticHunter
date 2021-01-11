#region Using directives

using System;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Bosses
{
	public class MoonLordSoul : PostHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.MoonLordCore;
		public override string soulDescription => "Summon laser shooting eyes.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 40;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;
			int damage = 300 + (20 * amount);

			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = Main.MouseWorld;

				// Hardcoded, since I don't have the good patience to figure it out atm.
				if (amount == 2)
					spawnPos += new Vector2(-46 * (i == 0 ? 1 : -1), 0);
				else if (amount == 3)
					spawnPos += new Vector2(-46 * (1 - i), Math.Abs(i - 1) * -46);

				Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<MoonLordSoulProj>(), damage, 0, p.whoAmI);
			}
			return (true);
		}
	}

	public class MoonLordSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.MoonLordFreeEye;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eye");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.scale = .4f;
			projectile.timeLeft = 600;

			projectile.ignoreWater = true;
			projectile.tileCollide = false;

			projectile.localNPCHitCooldown = 1;
			projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				DustEffect();
				projectile.localAI[0] = .5f;
				projectile.ai[0] = MathHelper.PiOver4;

				if (projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(projectile.Center, projectile.ai[0].ToRotationVector2() * 4, ModContent.ProjectileType<MoonLordSoulProjBeam>(),
						projectile.damage, projectile.knockBack, projectile.owner, 30f, projectile.whoAmI);
				}
			}
			else
			{
				projectile.ai[0] += (MathHelper.PiOver2 / 90);
				if (projectile.ai[0] > MathHelper.PiOver2 + MathHelper.PiOver4)
				{
					projectile.Kill();
				}
			}

			// Animation.
			if (projectile.frameCounter++ >= 4)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			return (false);
		}

		public override bool? CanDamage()
			=> false;

		public override void Kill(int timeLeft)
			=> DustEffect();

		private void DustEffect()
		{
			for (int i = 0; i < 10; ++i)
				Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.AncientLight,
					projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100, default, 1.5f)].noGravity = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{			
			Texture2D projTexture = TextureAssets.Projectile[Type].Value;
			Texture2D irisTexture = TextureAssets.Extra[19].Value;
			Vector2 offset = new Vector2(30f, 30f);

			Point p = projectile.Center.ToTileCoordinates();
			Color color = projectile.GetAlpha(Color.Lerp(Lighting.GetColor(p.X, p.Y), Color.White, .3f));
			Vector2 irisOffset = Utils.Vector2FromElipse(projectile.ai[0].ToRotationVector2(), offset * projectile.localAI[0]);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, projTexture.Frame(1, 4, 0, projectile.frame), color, projectile.rotation,
				new Vector2(40, 40), projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(irisTexture, projectile.Center - Main.screenPosition + irisOffset, null, color, projectile.rotation,
				irisTexture.Size() / 2, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}

	public class MoonLordSoulProjBeam : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PhantasmalDeathray;

		private readonly float MaxRange = 1000;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beam");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 36;

			projectile.alpha = 200;
			projectile.timeLeft = 90;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Projectile owningProjectile = Main.projectile[(int)projectile.ai[1]];

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
				projectile.velocity = -Vector2.UnitY;

			if (!owningProjectile.active)
				projectile.Kill();
			else
			{
				Vector2 centerOffset = new Vector2(30, 30);
				Vector2 rotationOffset = Utils.Vector2FromElipse(owningProjectile.ai[0].ToRotationVector2(), centerOffset * owningProjectile.scale);
				projectile.position = owningProjectile.Center + rotationOffset - new Vector2(projectile.width, projectile.height) * .5f;
			}

			if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
				projectile.velocity = -Vector2.UnitY;

			projectile.localAI[0]++;
			projectile.scale = (float)Math.Sin(projectile.localAI[0] * (float)Math.PI / 180) * 10 * .4f;
			if (projectile.scale > .4f)
				projectile.scale = .4f;

			float rotation = owningProjectile.ai[0];
			projectile.rotation = rotation - MathHelper.PiOver2;
			projectile.velocity = rotation.ToRotationVector2();

			Vector2 samplingPoint = projectile.Center;

			float[] laserScanSamples = new float[3];
			Collision.LaserScan(projectile.Center, projectile.velocity, projectile.width * projectile.scale, MaxRange, laserScanSamples);

			float endPointLength = laserScanSamples.Sum() / laserScanSamples.Length;
			projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], endPointLength, .5f);

			// Visual effects.
			Vector2 endPoint = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14);
			for (int i = 0; i < 2; i++)
			{
				float rot = projectile.rotation + ((Main.rand.Next(2) == 1) ? -1 : 1) * MathHelper.PiOver2;
				float randRot = Main.rand.NextFloat() * 2f + 2f;

				Vector2 dustVelocity = new Vector2((float)Math.Cos(rot) * randRot, (float)Math.Sin(rot) * randRot);
				Dust d = Main.dust[Dust.NewDust(endPoint, 0, 0, 229, dustVelocity.X, dustVelocity.Y, 0, default, 1.7f)];
				d.noGravity = true;
			}

			DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, DelegateMethods.CastLight);
			return (false);
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
			float collPoint = 0f;
			return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1],
				36 * projectile.scale, ref collPoint));
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.velocity == Vector2.Zero)
				return (false);

			Texture2D tex = TextureAssets.Projectile[Type].Value;
			Texture2D extraTex01 = TextureAssets.Extra[21].Value;
			Texture2D extraTex02 = TextureAssets.Extra[22].Value;
			float length = projectile.localAI[1];

			Color drawColor = new Color(255, 255, 255, 0) * 0.9f;
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, drawColor, projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

			length -= (tex.Height / 2 + extraTex02.Height) * projectile.scale;
			Vector2 center = projectile.Center + projectile.velocity * projectile.scale * tex.Height / 2;
			if (length > 0)
			{
				float currentLength = 0;
				Rectangle rectangle = new Rectangle(0, 16 * (projectile.timeLeft / 3 % 5), extraTex01.Width, 16);
				while (currentLength + 1 < length)
				{
					if (length - currentLength < rectangle.Height)
						rectangle.Height = (int)(length - currentLength);

					spriteBatch.Draw(extraTex01, center - Main.screenPosition, rectangle, drawColor, projectile.rotation,
						new Vector2(rectangle.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);
					currentLength += rectangle.Height * projectile.scale;

					center += projectile.velocity * rectangle.Height * projectile.scale;

					rectangle.Y += 16;
					if (rectangle.Y + rectangle.Height > extraTex01.Height)
						rectangle.Y = 0;
				}
			}

			Main.spriteBatch.Draw(extraTex02, center - Main.screenPosition, null, drawColor, projectile.rotation, tex.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
			return (false);
		}
	}
}
