using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System;

namespace MysticHunter.Souls.Data.Event.PirateInvasion
{
	public class FlyingDutchmanSoul : PostHMSoul, IBossSoul, IEventSoul
	{
		public override short soulNPC => NPCID.PirateShipCannon;
		public override string soulDescription => "Summons a heavy duty canon.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			int damage = 55;

			if (stack >= 5)
			{
				amount += 1;
				damage += 5;
			}
			if (stack >= 9)
			{
				amount += 2;
				damage += 5;
			}

			// Despawn pre-existing projectiles.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<FlyingDutchmanSoulProj>())
				{
					Main.projectile[i].Kill();
				}
			}

			int[] projectileXOffsets = { 100, -100, 164, -164 };
			for (int i = 0; i < amount; ++i)
			{
				Vector2 newProjPos = p.Center + new Vector2(projectileXOffsets[i] * p.direction, 0);
				if (Collision.SolidCollision(newProjPos, 16, 16))
					continue;

				Projectile.NewProjectile(newProjPos, Vector2.Zero, ProjectileType<FlyingDutchmanSoulProj>(), damage, 0, p.whoAmI);
			}
			return (true);
		}
	}

	public class FlyingDutchmanSoulProj : ModProjectile
	{
		private float wheelRotation = 0;

		private const float cannonballSpeed = 8;
		private const float cannonballGravity = 0.28f;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
			DisplayName.SetDefault("Cannon");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 28;

			projectile.timeLeft = 1800;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			int target = 0;
			bool hasTarget = false;
			float targetingRange = 600;

			// If the projectile doesn't have a target, search for one.
			if (projectile.ai[1] == 0)
			{
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].active &&/*Main.npc[i].CanBeChasedBy(projectile) && */(projectile.ai[1] == 0 || projectile.ai[1] == i + 1))
					{
						float targetDistance = projectile.Distance(Main.npc[i].Center);

						if (targetDistance < targetingRange && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
						{
							targetingRange = targetDistance;
							hasTarget = true;
							target = i;
						}
					}
				}

				if (hasTarget)
				{
					projectile.ai[1] = target + 1;
				}
				hasTarget = false;
			}
			// If the projectile has a target, check if it's still valid.
			if (projectile.ai[1] > 0)
			{
				NPC npc = Main.npc[(int)projectile.ai[1] - 1];

				if (/*npc.CanBeChasedBy(projectile, true) && */!npc.dontTakeDamage)
				{
					float targetDistance = projectile.Distance(npc.Center);
					if (targetDistance < 1000f)
					{
						hasTarget = true;
					}
				}
				else
				{
					projectile.ai[1] = 0;
				}
			}

			// If this projectile has a valid target, track it and shoot.
			if (hasTarget)
			{
				NPC npc = Main.npc[(int)projectile.ai[1] - 1];

				Vector2 targetDir = npc.Center - projectile.Center;
				targetDir.Y -= Math.Abs(targetDir.X) * cannonballGravity;
				float desiredRotation = targetDir.ToRotation();

				projectile.rotation = desiredRotation;
				projectile.spriteDirection = Math.Sign(npc.Center.X - projectile.Center.X);

				if (projectile.ai[0]++ >= 120)
				{
					projectile.ai[0] = 0;
					projectile.localAI[0] = 20;
					projectile.localAI[1] = projectile.spriteDirection;
					Projectile.NewProjectile(projectile.Center, Vector2.Normalize(targetDir) * cannonballSpeed, ProjectileID.CannonballFriendly, projectile.damage, projectile.knockBack, projectile.owner);
				}
			}

			projectile.frame = 0;
			if (projectile.localAI[0] > 0)
			{
				if (projectile.localAI[0] > 10)
				{
					projectile.frame = (int)((20 - projectile.localAI[0]) / 2);
					projectile.velocity.X = -projectile.spriteDirection * .5f;
				}
				else
				{
					projectile.velocity.X = projectile.spriteDirection * .5f;
				}
				projectile.localAI[0]--;
			}
			else
			{
				projectile.velocity.X = 0;
			}

			projectile.velocity.Y += .2f;
			wheelRotation += projectile.velocity.X * .06f;
			return (false);
		}

		public override bool? CanHitNPC(NPC target) => false;
		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}


		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = GetTexture(Texture);
			Texture2D wheelTexture = GetTexture("MysticHunter/Souls/Data/Event/PirateInvasion/FlyingDutchmanSoulProj_Wheel");

			Vector2 origin = new Vector2(12, 30);
			Vector2 drawOffset = new Vector2(0, -6);
			SpriteEffects effects = SpriteEffects.None;
			Rectangle frame = projTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

			if (projectile.spriteDirection == -1)
			{
				drawOffset.Y = 0;
				effects = SpriteEffects.FlipVertically;
			}

			spriteBatch.Draw(projTexture, projectile.position + drawOffset + projectile.Size/2 - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effects, 0);
			spriteBatch.Draw(wheelTexture, projectile.position + projectile.Size/2 - Main.screenPosition, null, lightColor, wheelRotation, wheelTexture.Size() / 2, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}
	}
}
