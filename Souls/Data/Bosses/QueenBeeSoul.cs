using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class QueenBeeSoul : PreHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.QueenBee;
		public override string soulDescription => "Summon an army of hornets.";

		public override short cooldown => 3600;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 40;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < 4; ++i)
			{
				int damage = 20 + 5 * stack;
				Vector2 spawnPosition = p.Center + Main.rand.NextVector2Unit() * 80;

				Projectile.NewProjectile(spawnPosition, Vector2.Zero, ProjectileType<QueenBeeSoulProj>(), damage, .1f, p.whoAmI);
			}
			return (true);
		}
	}

	public class QueenBeeSoulProj : ModProjectile
	{
		// No need to sync, just visually.
		private bool justSpawned;

		public override string Texture => "Terraria/Projectile_373";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hornet");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 26;

			projectile.timeLeft = 600;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			justSpawned = true;
		}

		public override bool PreAI()
		{
			if (justSpawned)
			{
				DustEffect();
				justSpawned = false;
			}
			float acceleration = 0.05f;
			float accelerationDist = (float)projectile.width;

			for (int m = 0; m < 1000; m++)
			{
				if (m != projectile.whoAmI &&
					Main.projectile[m].active &&
					Main.projectile[m].owner == projectile.owner &&
					Main.projectile[m].type == projectile.type &&
					Math.Abs(projectile.position.X - Main.projectile[m].position.X) + Math.Abs(projectile.position.Y - Main.projectile[m].position.Y) < accelerationDist)
				{
					if (projectile.position.X < Main.projectile[m].position.X)
						projectile.velocity.X -= acceleration;
					else
						projectile.velocity.X += acceleration;
					if (projectile.position.Y < Main.projectile[m].position.Y)
						projectile.velocity.Y -= acceleration;
					else
						projectile.velocity.Y += acceleration;
				}
			}
			Vector2 targetPosition = projectile.position;
			float distance = 400f;
			bool hasTarget = false;
			projectile.tileCollide = true;

			NPC ownerTarget = projectile.OwnerMinionAttackTargetNPC;
			if (ownerTarget != null && ownerTarget.CanBeChasedBy(this))
			{
				float currentDistance = Vector2.Distance(ownerTarget.Center, projectile.Center);

				if (((Vector2.Distance(projectile.Center, targetPosition) > currentDistance && currentDistance < distance) || !hasTarget) &&
					Collision.CanHitLine(projectile.position, projectile.width, projectile.height, ownerTarget.position, ownerTarget.width, ownerTarget.height))
				{
					distance = currentDistance;
					targetPosition = ownerTarget.Center;
					hasTarget = true;
				}
			}
			if (!hasTarget)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy())
					{
						float currentDistance = Vector2.Distance(npc.Center, projectile.Center);

						if (((Vector2.Distance(projectile.Center, targetPosition) > currentDistance && currentDistance < distance) || !hasTarget) &&
							Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							distance = currentDistance;
							targetPosition = npc.Center;
							hasTarget = true;
						}
					}
				}
			}

			int maxPlayerDistance = 500;
			if (hasTarget)
				maxPlayerDistance = 1000;

			Player player = Main.player[projectile.owner];
			float playerDistance = Vector2.Distance(player.Center, projectile.Center);
			if (playerDistance > maxPlayerDistance)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[0] == 1f)
				projectile.tileCollide = false;

			if (hasTarget && projectile.ai[0] == 0f)
			{
				Vector2 vector4 = targetPosition - projectile.Center;
				float length = vector4.Length();

				if (length > 200f)
				{
					float scaleFactor2 = 6f;
					vector4 = Vector2.Normalize(vector4) * scaleFactor2;
					projectile.velocity.X = (projectile.velocity.X * 40f + vector4.X) / 41f;
					projectile.velocity.Y = (projectile.velocity.Y * 40f + vector4.Y) / 41f;
				}
				else if (projectile.velocity.Y > -1f)
					projectile.velocity.Y -= 0.1f;
			}
			else
			{
				float speedToPlayer = 6f;

				if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
					projectile.ai[0] = 1f;

				if (projectile.ai[0] == 1f)
					speedToPlayer = 15f;

				Vector2 targetPlayerPos = player.Center - projectile.Center + new Vector2(0f, -60f);
				float length = targetPlayerPos.Length();

				if (length > 200f && speedToPlayer < 9f)
				{
					speedToPlayer = 9f;
				}
				if (length < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				if (length > 2000f)
				{
					projectile.position.X = player.Center.X - (projectile.width / 2);
					projectile.position.Y = player.Center.Y - (projectile.width / 2);
				}
				if (length > 70f)
				{
					targetPlayerPos = Vector2.Normalize(targetPlayerPos) * speedToPlayer;
					projectile.velocity = (projectile.velocity * 20f + targetPlayerPos) / 21f;
				}
				else
				{
					if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
					projectile.velocity *= 1.01f;
				}
			}

			// Visual effects.
			projectile.rotation = projectile.velocity.X * 0.05f;
			if (projectile.frameCounter++ >= 2)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			if (projectile.velocity.X > 0f)
				projectile.spriteDirection = projectile.direction = -1;
			else if (projectile.velocity.X < 0f)
				projectile.spriteDirection = projectile.direction = 1;

			if (projectile.ai[1] > 0f)
				projectile.ai[1] += Main.rand.Next(1, 4);
			if (projectile.ai[1] > 90f)
			{
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}

			if (projectile.ai[0] == 0f)
			{
				if (hasTarget)
				{
					if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					{
						if (projectile.ai[1] == 0f)
						{
							projectile.ai[1] += 1f;
							if (Main.myPlayer == projectile.owner)
							{
								Vector2 vel = Vector2.Normalize(targetPosition - projectile.Center) * 10;

								Projectile newProj = Main.projectile[Projectile.NewProjectile(projectile.Center, vel, 374, projectile.damage, 0f, Main.myPlayer)];
								newProj.timeLeft = 300;
								newProj.netUpdate = true;
								projectile.netUpdate = true;
							}
						}
					}
				}
			}
			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = true;
			return (true);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			DustEffect();
		}

		private void DustEffect()
		{
			for (int i = 0; i < 5; ++i)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Grass, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
