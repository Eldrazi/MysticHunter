using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class HerplingSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Herpling;
		public override string soulDescription => "Summons a mini Herpling.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 35;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 40;
			if (stack >= 5)
				damage += 10;
			if (stack >= 9)
				damage += 10;
			
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<HerplingSoulProj>())
					Main.projectile[i].Kill();
			}
			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<HerplingSoulProj>(), damage, .5f, p.whoAmI, -1);

			return (true);
		}
	}

	public class HerplingSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Herpling;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Herpling");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.Herpling];
		}
		public override void SetDefaults()
		{
			projectile.width = 58;
			projectile.height = 44;

			projectile.scale = .6f;
			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.netImportant = true;

			drawOffsetX = -20;
			drawOriginOffsetY = -4;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			// Check if the projectile should still be alive.
			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Blue].soulNPC == NPCID.Herpling)
				projectile.timeLeft = 2;

			MovementType movement = MovementType.None;
			bool flag3 = false;
			bool flag4 = false;

			int xOffset = 40 * owner.direction;
			if (owner.Center.X < projectile.Center.X - 10 + xOffset)
				movement = MovementType.Left;
			else if (owner.Center.X > projectile.Center.X + 10 + xOffset)
				movement = MovementType.Right;

			if (projectile.ai[1] == 0)
			{
				int maxDist = 500;
				if (projectile.localAI[0] > 0f)
					maxDist += 600;

				if (owner.rocketDelay2 > 0)
					projectile.ai[0] = 1f;

				Vector2 directionTowardsOwner = owner.Center - projectile.Center;
				float distance = directionTowardsOwner.Length();
				if (distance > 2000f)
				{
					projectile.position.X = owner.Center.X - projectile.width * 0.5f;
					projectile.position.Y = owner.Center.Y - projectile.height * 0.5f;
				}
				else if (distance > maxDist || (Math.Abs(directionTowardsOwner.Y) > 300f && projectile.localAI[0] <= 0f))
				{
					if (Math.Sign(directionTowardsOwner.Y) != Math.Sign(projectile.velocity.Y))
						projectile.velocity.Y = 0f;
					projectile.ai[0] = 1f;
				}
			}

			if (projectile.ai[0] != 0)
			{
				float acceleration = 0.2f;
				int num43 = 200;
				projectile.tileCollide = false;
				Vector2 moveDirection = owner.Center - projectile.Center;

				moveDirection.X -= 40 * owner.direction;
				float num45 = 700f;
				bool flag6 = false;
				int targetID = -1;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].CanBeChasedBy(this))
					{
						NPC target = Main.npc[i];
						float distance = projectile.Distance(target.Center);
						if (distance < num45)
						{
							if (Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
							{
								targetID = i;
							}
							flag6 = true;
							break;
						}
					}
				}
				if (!flag6)
					moveDirection.X -= 40 * owner.direction;

				if (flag6 && targetID >= 0)
					projectile.ai[0] = 0f;

				float num52 = moveDirection.Length();
				float num53 = 10f;

				if (num52 < num43 && owner.velocity.Y == 0f && projectile.position.Y + projectile.height <= owner.Center.Y &&
					!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					if (projectile.velocity.Y < -6f)
						projectile.velocity.Y = -6f;
				}
				if (num52 < 60f)
				{
					moveDirection = projectile.velocity;
				}
				else
				{
					num52 = num53 / num52;
					moveDirection *= num52;
				}

				if (projectile.velocity.X < moveDirection.X)
				{
					projectile.velocity.X += acceleration;
					if (projectile.velocity.X < 0f)
						projectile.velocity.X += acceleration * 1.5f;
				}
				if (projectile.velocity.X > moveDirection.X)
				{
					projectile.velocity.X -= acceleration;
					if (projectile.velocity.X > 0f)
						projectile.velocity.X -= acceleration * 1.5f;
				}
				if (projectile.velocity.Y < moveDirection.Y)
				{
					projectile.velocity.Y += acceleration;
					if (projectile.velocity.Y < 0f)
						projectile.velocity.Y += acceleration * 1.5f;
				}
				if (projectile.velocity.Y > moveDirection.Y)
				{
					projectile.velocity.Y -= acceleration;
					if (projectile.velocity.Y > 0f)
						projectile.velocity.Y -= acceleration * 1.5f;
				}

				projectile.frame = 0;
				projectile.rotation = projectile.velocity.X * 0.1f;
			}
			else
			{
				bool flag8 = false;

				int num88 = 60;
				projectile.localAI[0]--;
				if (projectile.localAI[0] < 0f)
					projectile.localAI[0] = 0f;

				if (projectile.ai[1] > 0f)
					projectile.ai[1]--;
				else
				{
					Vector2 targetPosition = projectile.position;
					float maxTargetingDistance = 100000f;
					float currentTargetingDistance = maxTargetingDistance;
					int targetID = -1;

					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC target = Main.npc[i];
						if (target.CanBeChasedBy(this))
						{
							float distToTarget = projectile.Distance(target.Center);
							if (distToTarget < currentTargetingDistance)
							{
								if (targetID == -1 && distToTarget <= maxTargetingDistance)
								{
									maxTargetingDistance = distToTarget;
									targetPosition = target.Center;
								}
								if (Collision.CanHit(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height))
								{
									currentTargetingDistance = distToTarget;
									targetPosition = target.Center;
									targetID = i;
								}
							}
						}
					}

					if (targetID == -1 && maxTargetingDistance < currentTargetingDistance)
						currentTargetingDistance = maxTargetingDistance;
					float num104 = 300f;
					if (projectile.position.Y > Main.worldSurface * 16.0f)
						num104 = 150f;

					if (currentTargetingDistance < num104 + xOffset && targetID == -1)
					{
						float xDiff = targetPosition.X - projectile.Center.X;
						if (xDiff < -5f)
							movement = MovementType.Left;
						else if (xDiff > 5f)
							movement = MovementType.Right;
					}
					bool flag9 = false;

					if (targetID >= 0 && currentTargetingDistance < 800f + xOffset)
					{
						projectile.friendly = true;
						projectile.localAI[0] = num88;

						float xDiff = targetPosition.X - projectile.Center.X;
						if (xDiff < -10f)
							movement = MovementType.Left;
						else if (xDiff > 10f)
							movement = MovementType.Right;

						if (targetPosition.Y < projectile.Center.Y - 100f && xDiff > -50f && xDiff < 50f && projectile.velocity.Y == 0f)
						{
							float yDiff = Math.Abs(targetPosition.Y - projectile.Center.Y);
							if (yDiff < 120f)
								projectile.velocity.Y = -10f;
							else if (yDiff < 210f)
								projectile.velocity.Y = -13f;
							else if (yDiff < 270f)
								projectile.velocity.Y = -15f;
							else if (yDiff < 310f)
								projectile.velocity.Y = -17f;
							else if (yDiff < 380f)
								projectile.velocity.Y = -18f;
						}
						if (flag9)
						{
							projectile.friendly = false;
							if (projectile.velocity.X < 0f)
								movement = MovementType.Left;
							else if (projectile.velocity.X > 0f)
								movement = MovementType.Right;
						}
					}
					else
						projectile.friendly = false;
				}

				if (projectile.ai[1] != 0f)
					movement = MovementType.None;

				if (!flag8)
					projectile.rotation = 0f;
				projectile.tileCollide = true;

				float acceleration = .2f;
				float speed = 6;

				if (speed < Math.Abs(owner.velocity.X) + Math.Abs(owner.velocity.Y))
				{
					acceleration = .3f;
					speed = Math.Abs(owner.velocity.X) + Math.Abs(owner.velocity.Y);
				}

				if (movement == MovementType.Left)
				{
					if (projectile.velocity.X > -3.5f)
						projectile.velocity.X -= acceleration;
					else
						projectile.velocity.X -= acceleration * .25f;
				}
				else if (movement == MovementType.Right)
				{
					if (projectile.velocity.X < 3.5f)
						projectile.velocity.X += acceleration;
					else
						projectile.velocity.X += acceleration * .25f;
				}
				else
				{
					projectile.velocity.X *= .9f;
					if (projectile.velocity.X >= -acceleration && projectile.velocity.X <= acceleration)
						projectile.velocity.X = 0f;
				}

				if (movement != MovementType.None)
				{
					int tileCheckX = (int)projectile.Center.X / 16 + (int)movement;
					int tileCheckY = (int)projectile.Center.Y / 16;

					tileCheckX += (int)projectile.velocity.X;
					if (WorldGen.SolidTile(tileCheckX, tileCheckY))
						flag4 = true;
				}

				if (owner.position.Y + owner.height - 8f > projectile.position.Y + projectile.height)
					flag3 = true;

				Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY);
				if (projectile.velocity.Y == 0f)
				{
					if (!flag3 && projectile.velocity.X != 0)
					{
						int tileCheckX = (int)projectile.Center.X / 16 + (int)movement;
						int tileCheckY = (int)projectile.Center.Y / 16 + 1;
						WorldGen.SolidTile(tileCheckX, tileCheckY);
					}

					if (flag4)
					{
						int tileCheckX = (int)projectile.Center.X / 16;
						int tileCheckY = (int)(projectile.position.Y + projectile.height) / 16 + 1;
						if (WorldGen.SolidTile(tileCheckX, tileCheckY) || Main.tile[tileCheckX, tileCheckY].halfBrick() || Main.tile[tileCheckX, tileCheckY].slope() > 0)
						{
							try
							{
								tileCheckX += (int)movement + (int)projectile.velocity.X;
								tileCheckY = (int)projectile.Center.Y / 16;

								if (!WorldGen.SolidTile(tileCheckX, tileCheckY - 1) && !WorldGen.SolidTile(tileCheckX, tileCheckY - 2))
									projectile.velocity.Y = -5.1f;
								else if (!WorldGen.SolidTile(tileCheckX, tileCheckY - 2))
									projectile.velocity.Y = -7.1f;
								else if (WorldGen.SolidTile(tileCheckX, tileCheckY - 5))
									projectile.velocity.Y = -11.1f;
								else if (WorldGen.SolidTile(tileCheckX, tileCheckY - 4))
									projectile.velocity.Y = -10.1f;
								else
									projectile.velocity.Y = -9.1f;
							}
							catch
							{
								projectile.velocity.Y = -9.1f;
							}
						}
					}
					else if (movement != MovementType.None)
						projectile.velocity.Y -= 6;
				}

				projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -speed, speed);
				projectile.direction = Math.Sign(projectile.velocity.X);

				if (projectile.velocity.X > acceleration && movement == MovementType.Right)
					projectile.direction = 1;
				if (projectile.velocity.X < -acceleration && movement == MovementType.Left)
					projectile.direction = -1;
				projectile.spriteDirection = -projectile.direction;

				if (projectile.velocity.Y >= 0f && projectile.velocity.Y <= 0.8f)
				{
					if (projectile.velocity.X == 0f)
						projectile.frameCounter++;
					else
						projectile.frameCounter += 3;
				}
				else
					projectile.frameCounter += 5;

				if (projectile.frameCounter >= 20)
				{
					projectile.frameCounter -= 20;
					projectile.frame++;
				}
				if (projectile.frame > 1)
					projectile.frame = 0;

				if (projectile.wet && owner.position.Y + owner.height < projectile.position.Y + projectile.height && projectile.localAI[0] == 0f)
				{
					if (projectile.velocity.Y > -4f)
						projectile.velocity.Y -= .2f;
					if (projectile.velocity.Y > 0f)
						projectile.velocity.Y *= .95f;
				}
				else
					projectile.velocity.Y += 0.4f;
				projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -10, 10);
			}

			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (projectile.tileCollide);
		}
		public override bool OnTileCollide(Vector2 oldVelocity) => false;
	}
}
