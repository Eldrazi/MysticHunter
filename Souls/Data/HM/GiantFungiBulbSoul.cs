#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class GiantFungiBulbSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.GiantFungiBulb;
		public override string soulDescription => "Summons a friendly Giant Fungi Bulb";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<GiantFungiBulbSoulProj>())
					Main.projectile[i].Kill();
			}

			int damage = 35;
			if (stack >= 5)
				damage += 10;
			if (stack >= 9)
				damage += 10;
			Projectile.NewProjectile(p.Center, default, ModContent.ProjectileType<GiantFungiBulbSoulProj>(), damage, .1f, p.whoAmI);
			return (true);
		}
	}

	public class GiantFungiBulbSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.GiantFungiBulb;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Fungi Bulb");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.GiantFungiBulb];
		}
		public override void SetDefaults()
		{
			projectile.width = 56;
			projectile.height = 47;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead || sp.BlueSoulNet.soulNPC == NPCID.GiantFungiBulb)
				projectile.timeLeft = 2;

			float maxSpeed = 2;
			float maxRange = 50 + (10 * sp.BlueSoulNet.stack);
			float acceleration = .055f;

			// Give a bit more range every 5 seconds.
			projectile.localAI[0]++;
			if (projectile.localAI[0] > 300)
			{
				maxRange *= 1.3f;
				if (projectile.localAI[0] > 450)
					projectile.localAI[0] = 0;
			}

			int targetIndex = 0;
			float currentTargetRange = Int32.MaxValue;
			// Fetch a target.
			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				float l = (Main.npc[i].Center - owner.Center).Length();
				if (Main.npc[i].CanBeChasedBy(projectile) && l <= currentTargetRange)
				{
					targetIndex = i;
					currentTargetRange = l;
				}
			}

			// Target following behavior.
			Vector2 targetDir;
			if (targetIndex != 0)
			{
				projectile.ai[0] = 0;
				projectile.ai[1] = 0;
				NPC target = Main.npc[targetIndex];

				targetDir = target.Center - owner.Center;
			}
			// If there is not NPC that can be targeted, we take a random point inside the maxRange.
			else
			{
				if (Main.myPlayer == owner.whoAmI && projectile.localAI[1] % 300 == 0)
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
			if (projectile.Center.X < owner.Center.X + targetDir.X)
			{
				projectile.velocity.X += acceleration;
				if (projectile.velocity.X < 0 && targetDir.X > 0)
					projectile.velocity.X += acceleration * 1.5f;
			}
			else if (projectile.Center.X > owner.Center.X + targetDir.X)
			{
				projectile.velocity.X -= acceleration;
				if (projectile.velocity.X > 0 && targetDir.X < 0)
					projectile.velocity.X -= acceleration * 1.5f;
			}

			if (projectile.Center.Y < owner.Center.Y + targetDir.Y)
			{
				projectile.velocity.Y += acceleration;
				if (projectile.velocity.Y < 0 && targetDir.Y > 0)
					projectile.velocity.Y += acceleration * 1.5f;
			}
			else if (projectile.Center.Y > owner.Center.Y + targetDir.Y)
			{
				projectile.velocity.Y -= acceleration;
				if (projectile.velocity.Y > 0 && targetDir.Y < 0)
					projectile.velocity.Y -= acceleration * 1.5f;
			}

			projectile.direction = 1;
			projectile.rotation = (owner.Center - projectile.Center).ToRotation();
			projectile.velocity = Vector2.Clamp(projectile.velocity, -Vector2.One * maxSpeed, Vector2.One * maxSpeed);

			// Special distance check.
			// If the projectile is too far from the player, we want to teleport it closer.
			if ((owner.Center - projectile.Center).Length() >= 600)
			{
				DustEffect();
				projectile.position = owner.Center + targetDir;
				DustEffect();
			}

			// Animation.
			if (projectile.frameCounter++ >= 7)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			return (false);
		}

		private void DustEffect()
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 40, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D chainTexture = TextureAssets.Chain25.Value;
			Player owner = Main.player[projectile.owner];

			Vector2 chainOrigin = new Vector2(chainTexture.Width *.5f, chainTexture.Height * .5f);

			Vector2 projectileCenter = projectile.Center;
			Vector2 relationalDirection = owner.Center - projectileCenter;
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
				relationalDirection = owner.Center - projectileCenter;

				Color c = Lighting.GetColor((int)projectileCenter.X / 16, (int)projectileCenter.Y / 16);
				spriteBatch.Draw(chainTexture, projectileCenter - Main.screenPosition, new Rectangle(0, 0, chainTexture.Width, chainHeight), c, relationalRotation, chainOrigin, 1, SpriteEffects.None, 0);
			}
			return (true);
		}
	}
}
