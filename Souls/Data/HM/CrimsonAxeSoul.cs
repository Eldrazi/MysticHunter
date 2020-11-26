using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class CrimsonAxeSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.CrimsonAxe;
		public override string soulDescription => "Summons guardian axes.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<CrimsonAxeSoulProj>())
					Main.projectile[i].Kill();

			int damage = 30;
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 4;
			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(p.Center, velocity, ProjectileType<CrimsonAxeSoulProj>(), damage, .2f, p.whoAmI, (int)WeaponSoulState.Idle);
			}
			return (true);
		}
	}

	internal enum WeaponSoulState : byte
	{
		Idle = 0,
		Attacking = 1,
		Recuperating = 2,
		RejoiningOwner = 3
	}

	public class CrimsonAxeSoulProj : ModProjectile
	{
		/// <summary>
		/// Swarm acceleration, which determines the 'push' applied to this projectile based on its position in relation to similar projectiles.
		/// </summary>
		private readonly float swarmAcceleration = .05f;

		/// <summary>
		/// The maximum allowed distance to the player when in <see cref="WeaponSoulState.RejoiningOwner"/> state.
		/// </summary>
		private readonly float maxPlayerDistance = 450f;

		/// <summary>
		/// The normal distance at which the projectile will start to rejoin its owner.
		/// </summary>
		private readonly float normalRejoinDistance = 900f;

		/// <summary>
		/// The distance at which the projectile will start to rejoin its owner if it has a target.
		/// This is generally higher than the <see cref="normalRejoinDistance"/>.
		/// </summary>
		private readonly float hasTargetRejoinDistance = 1500f;

		private WeaponSoulState AIState
		{
			get { return (WeaponSoulState)projectile.ai[0]; }
			set { projectile.ai[0] = (float)value; }
		}

		public override string Texture => "Terraria/NPC_179";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crimson Axe");
			Main.projFrames[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 40;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;

			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead || owner.GetModPlayer<SoulPlayer>().BlueSoulNet.soulNPC == NPCID.CrimsonAxe)
				projectile.timeLeft = 2;

			float targetingDistance = 1500f;

			// Projectile swarming.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (i != projectile.whoAmI && Main.projectile[i].type == projectile.type && Main.projectile[i].active && Main.projectile[i].owner == projectile.owner &&
					Math.Abs(projectile.position.X - Main.projectile[i].position.X) + Math.Abs(projectile.position.Y - Main.projectile[i].position.Y) < projectile.width)
				{
					if (projectile.position.X < Main.projectile[i].position.X)
						projectile.velocity.X -= swarmAcceleration;
					else
						projectile.velocity.X += swarmAcceleration;

					if (projectile.position.Y < Main.projectile[i].position.Y)
						projectile.velocity.Y -= swarmAcceleration;
					else
						projectile.velocity.Y += swarmAcceleration;
				}
			}

			if (AIState == WeaponSoulState.Recuperating)
			{
				projectile.velocity *= 0.9f;

				if (projectile.ai[1]++ > 16)
				{
					projectile.ai[1] = 0f;
					AIState = WeaponSoulState.Idle;
				}
			}
			else if (AIState == WeaponSoulState.Attacking)
			{
				projectile.MaxUpdates = 2;
				projectile.rotation += .4f;

				if (projectile.ai[1]++ > 60)
				{
					projectile.ai[1] = 1;
					projectile.numUpdates = 0;
					projectile.extraUpdates = 0;
					projectile.netUpdate = true;
					AIState = WeaponSoulState.Recuperating;
				}
			}
			else
			{
				Vector2 targetPosition = projectile.position;

				if (AIState != WeaponSoulState.RejoiningOwner)
					projectile.tileCollide = true;
				if ((projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16))) ||
					!Collision.CanHitLine(projectile.Center, 1, 1, owner.Center, owner.width, owner.height))
					projectile.tileCollide = false;

				bool hasTarget = false;
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy(projectile))
					{
						float distToNpc = Vector2.Distance(npc.Center, projectile.Center);

						if (((Vector2.Distance(projectile.Center, targetPosition) > distToNpc && distToNpc < targetingDistance) || !hasTarget) &&
						Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							hasTarget = true;
							targetPosition = npc.Center;
							targetingDistance = distToNpc;
						}
					}
				}

				float allowedDistance = normalRejoinDistance;
				if (hasTarget)
					allowedDistance = hasTargetRejoinDistance;
				if (Vector2.Distance(owner.Center, projectile.Center) > allowedDistance)
				{
					projectile.netUpdate = true;
					projectile.tileCollide = false;
					AIState = WeaponSoulState.RejoiningOwner;
				}

				float maxSpeed = 8f;
				if (AIState == WeaponSoulState.RejoiningOwner)
					maxSpeed = 13f;

				Vector2 directionTowardsPlayer = owner.Center - projectile.Center + new Vector2(0f, -60f);
				float distanceToPlayer = directionTowardsPlayer.Length();

				// State switch from RejoiningOwner -> Idle.
				if (distanceToPlayer < maxPlayerDistance && AIState == WeaponSoulState.RejoiningOwner &&
					!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.netUpdate = true;
					AIState = WeaponSoulState.Idle;
				}

				// Teleport to the owner if distance is too big.
				if (distanceToPlayer > 2000f)
				{
					projectile.netUpdate = true;
					projectile.position.X = owner.Center.X - (projectile.width * .5f);
					projectile.position.Y = owner.Center.Y - (projectile.height * .5f);
				}

				// Apply velocity towards owner.
				if (distanceToPlayer > 70f)
				{
					directionTowardsPlayer = Vector2.Normalize(directionTowardsPlayer) * maxSpeed;
					projectile.velocity = (projectile.velocity * 40f + directionTowardsPlayer) / 41f;
				}
				else if (projectile.velocity == Vector2.Zero)
					projectile.velocity = new Vector2(-.15f, -.05f);

				// Set correct rotation depending on state.
				if (AIState == WeaponSoulState.Idle)
				{
					float desiredRotation = (MathHelper.PiOver4 * 3) + (projectile.velocity.X * .15f);
					projectile.rotation = MathHelper.Lerp(projectile.rotation, desiredRotation, .05f);
				}
				else if (AIState == WeaponSoulState.RejoiningOwner)
					projectile.rotation += projectile.velocity.X * .04f;

				// Attack timer.
				if (projectile.ai[1] > 0f)
				{
					projectile.ai[1]++;
					if (projectile.ai[1] > 10)
					{
						projectile.ai[1] = 0f;
						projectile.netUpdate = true;
					}
				}

				// Set attacking state and update velocity accordingly.
				if (AIState == WeaponSoulState.Idle)
				{
					if (projectile.ai[1] == 0f && hasTarget && targetingDistance < 600)
					{
						projectile.ai[1] = 1;

						if (Main.myPlayer == owner.whoAmI)
						{
							projectile.netUpdate = true;
							AIState = WeaponSoulState.Attacking;
							projectile.velocity = Vector2.Normalize(targetPosition - projectile.Center) * 10f;
						}
					}
				}
			}

			// Visual effects.
			Lighting.AddLight(projectile.Center, 0.8f, 0.3f, 0.1f);
			if (projectile.frameCounter++ >= 6)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (true);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			bool collision = false;
			if (projectile.velocity.X != oldVelocity.X)
			{
				collision = true;
				projectile.velocity.X = -oldVelocity.X;
			}

			if (projectile.velocity.Y != oldVelocity.Y || projectile.velocity.Y == 0f)
			{
				collision = true;
				projectile.velocity.Y = -oldVelocity.Y * 0.5f;
			}

			if (collision)
			{
				float length = Math.Max(1, oldVelocity.Length() / projectile.velocity.Length());

				projectile.velocity /= length;
				if (projectile.ai[0] == 7f && projectile.velocity.Y < -.1f)
					projectile.velocity.Y += .1f;

				if (projectile.ai[0] >= 6f && projectile.ai[0] < 9f)
					Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
			}
			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = GetTexture(Texture);

			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, (projTexture.Height / Main.projFrames[projectile.type]) * .5f);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(projTexture, 1, Main.projFrames[projectile.type], 0, projectile.frame)),
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}
}
