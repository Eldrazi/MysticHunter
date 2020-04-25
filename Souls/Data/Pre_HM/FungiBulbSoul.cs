using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class FungiBulbSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.FungiBulb;
		public override string soulDescription => "Summons a friendly Fungi Bulb";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<FungiBulbSoulProj>())
					Main.projectile[i].Kill();
			}

			int damage = 10;
			if (stack >= 5)
				damage += 5;
			if (stack >= 9)
				damage += 5;
			Projectile.NewProjectile(p.Center, default, ProjectileType<FungiBulbSoulProj>(), damage, .1f, p.whoAmI);
			return (true);
		}
	}

	public class FungiBulbSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_259";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fungi Bulb");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 20;

			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player player = Main.player[projectile.owner];
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();

			// Kill the projectile if the soul is no longer available.
			if (player.dead || sp.activeSouls[(int)SoulType.Blue].soulNPC != NPCID.FungiBulb)
				projectile.Kill();
			// If we do not kill the projectile, we want to always keep it active.
			projectile.timeLeft = 10;

			float maxSpeed = 2;
			float maxRange = 50 + (10 * sp.activeSouls[(int)SoulType.Blue].stack);
			float acceleration = .055f;

			// Give a bit more range every 5 seconds.
			projectile.localAI[0]++;
			if (projectile.localAI[0] > 300)
			{
				maxRange *= 1.3f;
				if (projectile.localAI[0] > 450)
					projectile.localAI[0] = 0;
			}

			int targetIndex = 255;
			float currentTargetRange = Int32.MaxValue;
			// Fetch a target.
			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				float l = (Main.npc[i].Center - player.Center).Length();
				if (Main.npc[i].CanBeChasedBy(projectile) && l <= currentTargetRange)
				{
					targetIndex = i;
					currentTargetRange = l;
				}
			}

			// Target following behavior.
			Vector2 targetDir;
			if (targetIndex != 255)
			{
				projectile.ai[0] = 0;
				projectile.ai[1] = 0;
				NPC target = Main.npc[targetIndex];

				targetDir = target.Center - player.Center;
			}
			// If there is not NPC that can be targeted, we take a random point inside the maxRange.
			else
			{
				if (Main.myPlayer == player.whoAmI && projectile.localAI[1] % 300 == 0)
				{
					projectile.ai[0] = (Main.rand.Next((int)-maxRange, (int)maxRange + 1));
					projectile.ai[1] = (Main.rand.Next((int)-maxRange, (int)maxRange + 1));
					projectile.netUpdate = true;
				}
				projectile.localAI[1]++;
				targetDir = new Vector2(projectile.ai[0], projectile.ai[1]);
			}


			// Do not allow the npc to surpass the range barrier.
			float distanceToTarget = targetDir.Length();
			if (distanceToTarget > maxRange)
			{
				distanceToTarget = maxRange / distanceToTarget;
				targetDir *= distanceToTarget;
			}

			// Actual velocity calculation based on the target position.
			if (projectile.Center.X < player.Center.X + targetDir.X)
			{
				projectile.velocity.X += acceleration;
				if (projectile.velocity.X < 0 && targetDir.X > 0)
					projectile.velocity.X += acceleration * 1.5f;
			}
			else if (projectile.Center.X > player.Center.X + targetDir.X)
			{
				projectile.velocity.X -= acceleration;
				if (projectile.velocity.X > 0 && targetDir.X < 0)
					projectile.velocity.X -= acceleration * 1.5f;
			}

			if (projectile.Center.Y < player.Center.Y + targetDir.Y)
			{
				projectile.velocity.Y += acceleration;
				if (projectile.velocity.Y < 0 && targetDir.Y > 0)
					projectile.velocity.Y += acceleration * 1.5f;
			}
			else if (projectile.Center.Y > player.Center.Y + targetDir.Y)
			{
				projectile.velocity.Y -= acceleration;
				if (projectile.velocity.Y > 0 && targetDir.Y < 0)
					projectile.velocity.Y -= acceleration * 1.5f;
			}

			projectile.direction = 1;
			projectile.rotation = (projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;
			projectile.velocity = Vector2.Clamp(projectile.velocity, -Vector2.One * maxSpeed, Vector2.One * maxSpeed);

			// Special distance check.
			// If the projectile is too far from the player, we want to teleport it closer.
			if ((player.Center - projectile.Center).Length() >= 600)
			{
				DustEffect();
				projectile.position = player.Center + targetDir;
				DustEffect();
			}
			return (false);
		}

		private void DustEffect()
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 165, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D chainTexture = Main.chain24Texture;
			Player player = Main.player[projectile.owner];

			Vector2 chainOrigin = new Vector2(chainTexture.Width *.5f, chainTexture.Height * .5f);

			Vector2 projectileCenter = projectile.Center;
			Vector2 relationalDirection = player.Center - projectileCenter;
			float relationalRotation = relationalDirection.ToRotation() - MathHelper.PiOver2;

			bool drawChain = true;
			while (drawChain == true)
			{
				int chainHeight = 12;
				int minDrawDistance = 20;

				float relationalLength = relationalDirection.Length();
				if (relationalLength < minDrawDistance)
				{
					chainHeight = (int)relationalLength - minDrawDistance + chainHeight;
					drawChain = false;
				}

				relationalLength = chainHeight / relationalLength;

				relationalDirection *= relationalLength;
				projectileCenter += relationalDirection;
				relationalDirection = player.Center - projectileCenter;

				Color c = Lighting.GetColor((int)projectileCenter.X / 16, (int)projectileCenter.Y / 16);
				c.B = byte.MaxValue;
				if (c.R < 100)
					c.R = 100;
				if (c.G < 150)
					c.G = 150;

				spriteBatch.Draw(chainTexture, projectileCenter - Main.screenPosition, new Rectangle(0, 0, chainTexture.Width, chainHeight), c, relationalRotation, chainOrigin, 1, SpriteEffects.None, 0);
			}
			/*Vector2 vector2 = new Vector2(nPC.position.X + (float)(nPC.width / 2), nPC.position.Y + (float)(nPC.height / 2));
			float num6 = nPC.ai[0] * 16f + 8f - vector2.X;
			float num7 = nPC.ai[1] * 16f + 8f - vector2.Y;
			float rotation2 = (float)Math.Atan2((double)num7, (double)num6) - 1.57f;
			bool flag5 = true;
			while (flag5)
			{
				int num8 = 12;
				int num9 = 20;

				float num10 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
				if (num10 < (float)num9)
				{
					num8 = (int)num10 - num9 + num8;
					flag5 = false;
				}
				num10 = (float)num8 / num10;
				num6 *= num10;
				num7 *= num10;
				x = ref vector2.X;
				x += num6;
				x = ref vector2.Y;
				x += num7;
				num6 = nPC.ai[0] * 16f + 8f - vector2.X;
				num7 = nPC.ai[1] * 16f + 8f - vector2.Y;*/

			return (true);
		}
	}
}
