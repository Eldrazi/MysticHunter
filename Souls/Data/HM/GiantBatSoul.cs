using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class GiantBatSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.GiantBat;
		public override string soulDescription => "Summons a Giant Bat.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<GiantBatSoulProj>())
					Main.projectile[i].Kill();
			}

			Projectile.NewProjectile(p.Center, Vector2.UnitX, ProjectileType<GiantBatSoulProj>(), 35 + 2 * stack, .1f, p.whoAmI, stack);
			return (true);
		}
	}

	public class GiantBatSoulProj : ModProjectile
	{
		// No need to sync, just visually.
		private bool justSpawned;

		public override string Texture => "Terraria/NPC_93";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Bat");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 26;

			projectile.timeLeft = 600;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			justSpawned = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead || owner.GetModPlayer<SoulPlayer>().BlueSoul?.soulNPC == NPCID.GiantBat)
				projectile.timeLeft = 2;

			if (justSpawned)
			{
				DustEffect();
				justSpawned = false;
			}

			Vector2 targetPosition = projectile.position;
			float distance = 400f;
			bool hasTarget = false;
			projectile.tileCollide = true;

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

			int maxPlayerDistance = 500;
			if (hasTarget)
				maxPlayerDistance = 1000;

			float playerDistance = Vector2.Distance(owner.Center, projectile.Center);
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

				if (length > 10f)
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

				if (!Collision.CanHitLine(projectile.Center, 1, 1, owner.Center, 1, 1))
					projectile.ai[0] = 1f;

				if (projectile.ai[0] == 1f)
					speedToPlayer = 15f;

				Vector2 targetPlayerPos = owner.Center - projectile.Center + new Vector2(0f, -60f);
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
					projectile.position.X = owner.Center.X - (projectile.width / 2);
					projectile.position.Y = owner.Center.Y - (projectile.width / 2);
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
			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = true;
			return (true);
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void Kill(int timeLeft) => DustEffect();

		private void DustEffect()
		{
			for (int i = 0; i < 5; ++i)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
