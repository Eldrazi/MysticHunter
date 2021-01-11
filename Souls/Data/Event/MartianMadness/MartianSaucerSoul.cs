#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class MartianSaucerSoul : PostHMSoul, IBossSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MartianSaucer;
		public override string soulDescription => "Summon a mini saucer with a big beam.";

		public override short cooldown => 720;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 90 + 10 * stack;

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<MartianSaucerSoulProj>(), damage, 1f, p.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
			return (true);
		}
	}

	internal sealed class MartianSaucerSoulProj : ModProjectile
	{
		private readonly float speed = 6;
		private readonly float acceleration = .08f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mini Saucer");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.timeLeft = 480;

			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;

			drawOffsetX = -7;
			drawOriginOffsetY = -2;
		}

		public override bool PreAI()
		{
			Vector2 targetPosition = new Vector2(projectile.ai[0], projectile.ai[1]);

			Vector2 center = projectile.Center;

			if (projectile.Center.X < targetPosition.X)
			{
				projectile.velocity.X += acceleration;
				if (projectile.velocity.X < 0)
				{
					projectile.velocity.X *= .98f;
				}
			}
			else
			{
				projectile.velocity.X -= acceleration;
				if (projectile.velocity.X > 0)
				{
					projectile.velocity.X *= .98f;
				}
			}

			if (projectile.Center.Y < targetPosition.Y)
			{
				projectile.velocity.Y += acceleration;
				if (projectile.velocity.Y < 0)
				{
					projectile.velocity.Y *= .98f;
				}
			}
			else
			{
				projectile.velocity.Y -= acceleration;
				if (projectile.velocity.Y > 0)
				{
					projectile.velocity.Y *= .98f;
				}
			}

			projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -speed, speed);
			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -speed, speed);

			if (Main.myPlayer == projectile.whoAmI)
			{
				if (projectile.localAI[0] == 0 && projectile.Distance(targetPosition) <= 64)
				{
					projectile.localAI[0] = 1;
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MartianSaucerSoulProj_Laser>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.whoAmI + 1);
				}
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}

		public override bool? CanHitNPC(NPC target) => false;
	}

	internal sealed class MartianSaucerSoulProj_Laser : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SaucerDeathray;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Deathray");
			Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.SaucerDeathray];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 30;

			projectile.scale = .5f;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Projectile ownerProjectile = Main.projectile[(int)projectile.ai[0] - 1];

			if (projectile.ai[1] == 0)
			{
				projectile.ai[1] = 1;
				SoundEngine.PlaySound(SoundID.Item12, projectile.Center);
			}

			if (!ownerProjectile.active || ownerProjectile.owner != projectile.owner)
			{
				projectile.Kill();
				return (false);
			}

			float yPos = ownerProjectile.Center.Y + 16;

			int ownerTileXPos = (int)ownerProjectile.Center.X / 16;
			int ownerTileYPos = (int)yPos / 16;

			int yCol = 0;

			if (!Main.tile[ownerTileXPos, ownerTileYPos].nactive() || !Main.tileSolid[Main.tile[ownerTileXPos, ownerTileYPos].type] || Main.tileSolidTop[Main.tile[ownerTileXPos, ownerTileYPos].type])
			{
				while (yCol < 150 && ownerTileYPos + yCol < Main.maxTilesY)
				{
					int curY = ownerTileYPos + yCol;

					if (Main.tile[ownerTileXPos, curY].nactive() && Main.tileSolid[Main.tile[ownerTileXPos, curY].type] && !Main.tileSolidTop[Main.tile[ownerTileXPos, curY].type])
					{
						yCol--;
						break;
					}
					yCol++;

				}
			}
			else
			{
				yCol = 1;
			}

			projectile.position.X = ownerProjectile.Center.X - (projectile.width / 2);
			projectile.position.Y = yPos;
			projectile.height = (yCol + 1) * 16;

			int bottom = (int)projectile.position.Y + projectile.height;
			if (Main.tile[ownerTileXPos, bottom / 16].nactive() && Main.tileSolid[Main.tile[ownerTileXPos, bottom / 16].type] && !Main.tileSolidTop[Main.tile[ownerTileXPos, bottom / 16].type])
			{
				projectile.height -= (bottom % 16) - 2;
			}

			// Dust effects.
			for (int i = 0; i < 2; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position + new Vector2(0, projectile.height - 16f), projectile.width, 16, 228)];
				newDust.noGravity = true;
				newDust.velocity *= .5f;
				newDust.velocity.X -= (i - ownerProjectile.velocity.X * 2f / 3f);
				newDust.scale = 2.8f;
			}
			if (Main.rand.Next(5) == 0)
			{
				Vector2 newDustPos = projectile.position + new Vector2(
					projectile.width / 2 - (projectile.width / 2 * Math.Sign(ownerProjectile.velocity.X)) - 4f,
					projectile.height - 16);

				Dust newDust = Main.dust[Dust.NewDust(newDustPos, 4, 16, 31, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= .5f;
				newDust.velocity.X -= ownerProjectile.velocity.X / 2f;
				newDust.velocity.Y *= -1;
			}

			// Animation.
			if (++projectile.frameCounter >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Texture2D extraTexture = TextureAssets.Extra[4].Value;

			int frameHeight = texture.Height / Main.projFrames[projectile.type]; // num219
			int extraFrameHeight = extraTexture.Height / Main.projFrames[projectile.type]; // num220

			int frameY = projectile.frame * frameHeight;
			int extraFrameY = projectile.frame * extraFrameHeight;

			Rectangle extraFrame = extraTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
			Vector2 extraDrawPosition = projectile.position + new Vector2(projectile.width / 2, 0) + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;

			Main.spriteBatch.Draw(extraTexture, extraDrawPosition, extraFrame, projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(extraTexture.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);

			int height = projectile.height - frameHeight - 6;
			if (height < 0)
			{
				height = 0;
			}
			else if (height > 0)
			{
				if (extraFrameY == extraFrameHeight * 3)
				{
					extraFrameY = extraFrameHeight * 2;
				}
				Main.spriteBatch.Draw(extraTexture, extraDrawPosition + Vector2.UnitY * ((extraFrameHeight - 1) * projectile.scale), new Rectangle(0, extraFrameY + extraFrameHeight - 1, extraTexture.Width, 1),
					projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(extraTexture.Width / 2, 0), new Vector2(projectile.scale, height), SpriteEffects.None, 0f);
			}

			extraFrame.Y = frameY;
			extraFrame.Width = texture.Width;
			Main.spriteBatch.Draw(texture, extraDrawPosition + Vector2.UnitY * (extraFrameHeight + height - 1), extraFrame,
				projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2, 0), projectile.scale, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
