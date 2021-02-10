#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class StardustPillarSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.LunarTowerStardust;
		public override string soulDescription => "Summons a Summon boosting mini Stardust Pillar.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<StardustPillarSoul_Proj>())
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			Projectile.NewProjectile(p.Center, default, ModContent.ProjectileType<StardustPillarSoul_Proj>(), 200, 0, p.whoAmI, stack);

			return (true);
		}
	}

	public class StardustPillarSoul_Proj : ModProjectile
	{
		private readonly float minSpeed = 0.1f, maxSpeed = 10f;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardust Pillar");
			Main.projFrames[projectile.type] = 4;

			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 36;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.LunarTowerStardust)
			{
				projectile.timeLeft = 2;

				// Boost owner's ranged stats.
				owner.statManaMax2 += 10 + 5 * (int)projectile.ai[0];
				owner.minionDamage += 0.2f + (0.05f * projectile.ai[0]);
			}

			// Special distance check.
			// If the projectile is too far from the player, we want to teleport it closer.
			if (projectile.Distance(owner.Center) >= 600)
			{
				DustEffect();
				projectile.position = owner.Center;
				DustEffect();
			}

			// Movement

			Vector2 targetDirection = owner.Center + new Vector2(0, -60);
			targetDirection -= projectile.Center;

			float speed = MathHelper.Lerp(minSpeed, maxSpeed, targetDirection.Length() / 600);
			float acceleration = 0.1f;

			targetDirection = targetDirection.SafeNormalize(Vector2.Zero) * speed;

			if (projectile.velocity.X < targetDirection.X)
			{
				if (projectile.velocity.X < 0)
				{
					projectile.velocity.X *= 0.98f;
				}
				projectile.velocity.X += acceleration;
			}
			else if (projectile.velocity.X > targetDirection.X)
			{
				if (projectile.velocity.X > 0)
				{
					projectile.velocity.X *= 0.98f;
				}
				projectile.velocity.X -= acceleration;
			}

			if (projectile.velocity.Y < targetDirection.Y)
			{
				if (projectile.velocity.Y < 0)
				{
					projectile.velocity.Y *= 0.99f;
				}
				projectile.velocity.Y += acceleration;
			}
			else if (projectile.velocity.Y > targetDirection.Y)
			{
				if (projectile.velocity.Y > 0)
				{
					projectile.velocity.Y *= 0.99f;
				}
				projectile.velocity.Y -= acceleration;
			}
			
			// Shooting
			if (++projectile.ai[1] >= 120)
			{
				if (owner.whoAmI == Main.myPlayer)
				{
					int targetsAmount = 0;

					for (int i = 0; i < Main.maxNPCs && targetsAmount < 3; ++i)
					{
						NPC npc = Main.npc[i];

						Vector2 directionTowardsNPC = npc.Center - projectile.Center;
						if (!npc.active || !npc.CanBeChasedBy(projectile) ||
							!Collision.CanHitLine(projectile.Center, 1, 1, npc.Center, 1, 1) || directionTowardsNPC.Length() >= 600)
						{
							continue;
						}

						targetsAmount++;
						Projectile.NewProjectileDirect(projectile.Center, Vector2.Normalize(directionTowardsNPC) * 4f,
							ModContent.ProjectileType<StardustPillarSoul_ProjLaser>(), projectile.damage, 1f, projectile.owner);
					}

					if (targetsAmount != 0)
					{
						projectile.ai[1] = 0;
						projectile.netUpdate = true;
					}
				}
			}

			// Animation.
			if (projectile.frameCounter++ >= 7)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			projectile.rotation = projectile.velocity.X * 0.2f;
			return (false);
		}

		public override bool CanDamage()
			=> false;

		public override void Kill(int timeLeft)
			=> DustEffect();

		private void DustEffect()
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			this.DrawProjectileTrailCentered(spriteBatch, lightColor);

			return this.DrawAroundOrigin(spriteBatch, lightColor);
		}
	}

	public class StardustPillarSoul_ProjLaser : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.StardustJellyfishSmall;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.MinionShot[projectile.type] = true;
			Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.StardustJellyfishSmall];
		}
		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.StardustJellyfishSmall);

			projectile.aiStyle = 0;

			projectile.hostile = false;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (++projectile.frameCounter >= 2)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}

			if (Main.rand.Next(2) == 0)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 180, 0f, 0f, 100);
				newDust.scale += Main.rand.Next(50) * 0.01f;
				newDust.noGravity = true;
				newDust.velocity *= 0.1f;
				newDust.fadeIn = Main.rand.NextFloat() * 1.5f;
			}
			if (Main.rand.Next(3) == 0)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 176, 0f, 0f, 100);
				newDust.scale += 0.3f + Main.rand.Next(50) * 0.01f;
				newDust.noGravity = true;
				newDust.velocity *= 0.1f;
				newDust.fadeIn = Main.rand.NextFloat() * 1.5f;
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			projectile.position = projectile.Center;
			projectile.width = projectile.height = 80;
			projectile.Center = projectile.position;

			projectile.Damage();
			Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 7);

			for (int i = 0; i < 4; i++)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
				newDust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
			}
			for (int i = 0; i < 20; i++)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 176, 0f, 0f, 200, default, 3.7f);
				newDust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
				newDust.noGravity = true;
				newDust.velocity *= 3f;
			}
			for (int i = 0; i < 20; i++)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 180, 0f, 0f, 0, default, 2.7f);
				newDust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 2f;
				newDust.noGravity = true;
				newDust.velocity *= 3f;
			}
			for (int i = 0; i < 10; i++)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 0, default, 1.5f);
				newDust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 2f;
				newDust.noGravity = true;
				newDust.velocity *= 3f;
			}
		}
	}
}
