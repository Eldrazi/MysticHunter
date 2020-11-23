using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class DukeFishronSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DukeFishron;
		public override string soulDescription => "Summon giant waternado.";

		public override short cooldown => 1800;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 spawnPos = Main.MouseWorld;

			int maxIterations = 100;
			int x = (int)spawnPos.X / 16;
			int y = (int)spawnPos.Y / 16;
			while (maxIterations > 0)
			{
				if (Main.tile[x, y] == null)
				{
					y++;
					maxIterations--;
					continue;
				}
				if (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type])
					break;

				y++;
				maxIterations++;
			}

			spawnPos.Y = y * 16;

			Projectile.NewProjectile(spawnPos, new Vector2(Math.Sign(p.Center.X - spawnPos.X) * .01f, 0), ProjectileType<DukeFishronSoulProj>(), 170 + 5 * stack, 4, p.whoAmI, 16, 15);
			return (true);
		}
	}

	public class DukeFishronSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.Sharknado;

		private const int DefaultWidth = 150;
		private const int DefaultHeight = 42;

		int origX = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Waternado");
			Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.Sharknado];
		}
		public override void SetDefaults()
		{
			projectile.width = DefaultWidth;
			projectile.height = DefaultHeight;

			projectile.alpha = 255;
			projectile.timeLeft = 540;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			int num0 = 10;
			int num1 = 15;
			float scaleModifier = 1f;

			if (projectile.velocity.X != 0)
				projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);

			if (projectile.frameCounter++ >= 3)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			if (projectile.localAI[0] == 0 && projectile.owner == Main.myPlayer)
			{
				origX = (int)projectile.Center.X;
				projectile.localAI[0] = 1;

				projectile.position.X = projectile.position.X + (projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (projectile.height / 2);

				projectile.scale = ((num0 + num1) - projectile.ai[1]) * scaleModifier / (num0 + num1);
				projectile.width = (int)(DefaultWidth * projectile.scale);
				projectile.height = (int)(DefaultHeight * projectile.scale);

				projectile.position = projectile.position - new Vector2(projectile.width / 2, projectile.height / 2);
				projectile.netUpdate = true;
			}

			if (projectile.ai[1] != -1)
			{
				projectile.scale = ((num0 + num1) - projectile.ai[1]) * scaleModifier / (num0 + num1);
				projectile.width = (int)(DefaultWidth * projectile.scale);
				projectile.height = (int)(DefaultHeight * projectile.scale);
			}

			if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
			{
				projectile.alpha -= 30;
				if (projectile.alpha < 60)
					projectile.alpha = 60;
			}
			else
			{
				projectile.alpha += 30;
				if (projectile.alpha > 150)
					projectile.alpha = 150;
			}

			if (projectile.ai[0] > 0)
				projectile.ai[0] -= 1;
			if (projectile.ai[0] == 1 && projectile.ai[1] > 0 && projectile.owner == Main.myPlayer)
			{
				projectile.netUpdate = true;

				Vector2 center = new Vector2(origX, projectile.Center.Y);
				center.Y -= DefaultHeight * projectile.scale / 2;
				float newProjScaleModifier = ((num0 + num1) - projectile.ai[1] + 1) * scaleModifier / (num0 + num1);
				center.Y -= DefaultHeight * newProjScaleModifier / 2 - 2;

				Projectile.NewProjectile(center, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10, projectile.ai[1] - 1);
			}

			if (projectile.ai[0] <= 0)
			{
				float rotOffset = 0.104719758f;
				float widthMod = projectile.width * .2f;

				float xOffset = (float)(Math.Cos(rotOffset * -projectile.ai[0]) - .5) * widthMod;
				projectile.position.X -= xOffset * -projectile.direction;
				projectile.ai[0] -= 1;
				projectile.position.X += xOffset * -projectile.direction;
			}

			if (projectile.localAI[1]++ < 30)
				projectile.position.X += .5f * projectile.scale;
			else
			{
				projectile.position.X -= .5f * projectile.scale;
				if (projectile.localAI[1] >= 60)
				{
					projectile.localAI[1] = 0;
					projectile.netUpdate = true;
				}
			}

			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle frame = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
				frame,
				projectile.GetAlpha(lightColor),
				projectile.rotation,
				frame.Size() / 2,
				projectile.scale,
				projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

			return (false);
		}
	}
}
