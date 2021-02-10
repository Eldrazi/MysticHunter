#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class MothronEggSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MothronEgg;
		public override string soulDescription => "Toss an egg with a change to spawn a baby.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30 + 10 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.position) * 8f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<MothronEggSoul_Proj>(), damage, 1f, p.whoAmI, stack);

			return (true);
		}
	}

	public class MothronEggSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.MothronEgg;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Egg");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.scale = 0.5f;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1]++ >= 10)
			{
				projectile.velocity.X *= 0.98f;
				projectile.velocity.Y += 0.2f;
			}

			projectile.rotation += projectile.velocity.X * 0.1f;
			
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Web, 0f, 0f, 100);
			}

			if (projectile.owner == Main.myPlayer)
			{
				if (Main.rand.Next(1, 11) <= (int)projectile.ai[0])
				{
					Projectile.NewProjectile(projectile.position, Vector2.Zero, ModContent.ProjectileType<MothronEggSoul_EggSpawn>(), projectile.damage, projectile.knockBack, projectile.owner);
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor * projectile.Opacity);
	}

	public class MothronEggSoul_EggSpawn : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.MothronSpawn;

		private readonly float maxSpeed = 8f;
		private readonly float acceleration = 0.08f;
		private readonly float maxDistanceToTarget = 600;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.MothronSpawn];

			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.timeLeft = 600;

			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			Vector2 targetPosition = owner.position;

			float distanceToTarget = maxDistanceToTarget;

			for (byte i = 0; i < Main.maxNPCs; ++i)
			{
				NPC npc = Main.npc[i];
				if (!npc.active || !npc.CanBeChasedBy(projectile) || !Collision.CanHitLine(owner.position, 1, 1, npc.Center, 1, 1))
				{
					continue;
				}

				Vector2 directionTowards = (npc.Center - projectile.Center);
				if (directionTowards.Length() < distanceToTarget)
				{
					targetPosition = npc.Center;
					distanceToTarget = directionTowards.Length();
				}
			}

			targetPosition = Vector2.Normalize(targetPosition - projectile.Center) * maxSpeed;

			if (projectile.velocity.X < targetPosition.X)
			{
				if (projectile.velocity.X < 0)
				{
					projectile.velocity.X *= 0.98f;
				}
				projectile.velocity.X += acceleration;
			}
			else if (projectile.velocity.X > targetPosition.X)
			{
				if (projectile.velocity.X > 0)
				{
					projectile.velocity.X *= 0.98f;
				}
				projectile.velocity.X -= acceleration;
			}

			if (projectile.velocity.Y < targetPosition.Y)
			{
				if (projectile.velocity.Y < 0)
				{
					projectile.velocity.Y *= 0.99f;
				}
				projectile.velocity.Y += acceleration;
			}
			else if (projectile.velocity.Y > targetPosition.Y)
			{
				if (projectile.velocity.Y > 0)
				{
					projectile.velocity.Y *= 0.99f;
				}
				projectile.velocity.Y -= acceleration;
			}

			if (++projectile.frameCounter >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			projectile.rotation = projectile.velocity.X * 0.1f;
			projectile.spriteDirection = -projectile.direction;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			this.DrawProjectileTrailCentered(spriteBatch, lightColor);

			return this.DrawAroundOrigin(spriteBatch, lightColor * projectile.Opacity);
		}
	}
}
