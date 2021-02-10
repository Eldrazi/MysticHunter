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
	public class NebulaPillarSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.LunarTowerNebula;
		public override string soulDescription => "Summons a Magic boosting mini Nebula Pillar.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<NebulaPillarSoul_Proj>())
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			Projectile.NewProjectile(p.Center, default, ModContent.ProjectileType<NebulaPillarSoul_Proj>(), 200, 0, p.whoAmI, stack);

			return (true);
		}
	}

	public class NebulaPillarSoul_Proj : ModProjectile
	{
		private readonly float minSpeed = 0.1f, maxSpeed = 10f;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nebula Pillar");
			Main.projFrames[projectile.type] = 4;

			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 36;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.LunarTowerNebula)
			{
				projectile.timeLeft = 2;

				// Boost owner's ranged stats.
				owner.statManaMax2 += 20 + 5 * (int)projectile.ai[0];
				owner.magicDamage += 0.2f + (0.05f * projectile.ai[0]);
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
						Projectile.NewProjectileDirect(projectile.Center, Vector2.Normalize(directionTowardsNPC) * 12,
							ModContent.ProjectileType<NebulaPillarSoul_ProjLaser>(), projectile.damage, 1f, projectile.owner);
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
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 242, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			this.DrawProjectileTrailCentered(spriteBatch, lightColor);

			return this.DrawAroundOrigin(spriteBatch, lightColor);
		}
	}

	public class NebulaPillarSoul_ProjLaser : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.NebulaLaser;

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.NebulaLaser);

			aiType = ProjectileID.NebulaLaser;

			projectile.magic = true;
			projectile.hostile = false;
			projectile.friendly = true;
		}
	}
}
