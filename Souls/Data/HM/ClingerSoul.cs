using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Data.HM
{
	public class ClingerSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Clinger;
		public override string soulDescription => "Summons a friendly Clinger";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<ClingerSoulProj>())
					Main.projectile[i].Kill();
			}

			int damage = 15;
			if (stack >= 5)
				damage += 5;
			if (stack >= 9)
				damage += 5;
			Projectile.NewProjectile(p.Center, default, ProjectileType<ClingerSoulProj>(), damage, .1f, p.whoAmI);
			return (true);
		}
	}

	public class ClingerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_101";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clinger");
			Main.projFrames[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 30;

			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			// Kill the projectile if the player state is no longer valid, otherwise keep it alive.
			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.Clinger)
				projectile.timeLeft = 2;

			float maxSpeed = 4;
			float acceleration = .08f;
			float maxRange = 50 + (10 * sp.BlueSoulNet.stack);

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
				float l = (Main.npc[i].Center - owner.Center).Length();
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

				targetDir = (target.Center - owner.Center) * .8f;

				if (Main.myPlayer == projectile.owner && projectile.localAI[0] == 400)
				{
					Vector2 newProjectileVelocity = Vector2.Normalize(targetDir) * 5f;
					Projectile.NewProjectile(projectile.Center, newProjectileVelocity, ProjectileID.CursedFlameFriendly, projectile.damage, .2f, projectile.owner);
				}
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
			if (projectile.Distance(owner.Center) >= 600)
			{
				DustEffect();
				projectile.netUpdate = true;
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

		public override bool CanDamage() => false;

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
			Texture2D[] chainTextures = new Texture2D[] { Main.chain10Texture, Main.chain11Texture };
			Player owner = Main.player[projectile.owner];

			Vector2 chainOrigin = new Vector2(chainTextures[0].Width *.5f, chainTextures[0].Height * .5f);

			Vector2 projectileCenter = projectile.Center;
			Vector2 relationalDirection = owner.Center - projectileCenter;
			float relationalRotation = relationalDirection.ToRotation() - MathHelper.PiOver2;

			bool drawChain = true;
			int chainToDraw = 0;
			while (drawChain == true)
			{
				float scale = 0.75f;
				int chainHeight = 28;
				int minDrawDistance = 40;

				float relationalLength = relationalDirection.Length();
				if (relationalLength < chainHeight * scale)
				{
					chainHeight = (int)relationalLength - minDrawDistance + chainHeight;
					drawChain = false;
				}

				relationalLength = 20 * scale / relationalLength;

				relationalDirection *= relationalLength;
				projectileCenter += relationalDirection;
				relationalDirection = owner.Center - projectileCenter;

				Color c = Lighting.GetColor((int)projectileCenter.X / 16, (int)projectileCenter.Y / 16);
				spriteBatch.Draw(chainTextures[chainToDraw], projectileCenter - Main.screenPosition, new Rectangle(0, 0, chainTextures[0].Width, chainHeight), c, relationalRotation, chainOrigin, scale, SpriteEffects.None, 0);
				chainToDraw = (chainToDraw + 1) % 2;
			}

			Texture2D projTexture = GetTexture(Texture);

			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, (projTexture.Height / Main.projFrames[projectile.type]) * .5f);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(projTexture, 1, Main.projFrames[projectile.type], 0, projectile.frame)),
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}
}
