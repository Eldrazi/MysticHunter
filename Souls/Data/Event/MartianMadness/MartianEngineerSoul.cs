#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class MartianEngineerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MartianEngineer;
		public override string soulDescription => "Construct zapping Tesla Turrets.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 2 * stack;
			int damage = 15 + 2 * stack;

			// Despawn any pre-existing projectiles.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<MartianEngineerSoulProj>())
				{
					Main.projectile[i].Kill();
				}
			}
			
			int projectileXStart = 48;
			int projectileXSpacing = 24;
			for (int i = 0; i < amount; ++i)
			{
				int side = i % 2 == 0 ? -1 : 1;
				Vector2 newProjPos = p.position + new Vector2((projectileXStart + projectileXSpacing * i) * side, -16);

				if (Collision.SolidCollision(newProjPos, 16, 16))
					continue;

				Projectile.NewProjectile(newProjPos, Vector2.Zero, ModContent.ProjectileType<MartianEngineerSoulProj>(), damage, 0, p.whoAmI);
			}

			return (true);
		}
	}

	internal sealed class MartianEngineerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.MartianTurret;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tesla Turret");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.MartianTurret];
		}
		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 50;

			projectile.timeLeft = 600;

			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			// Setup the projectile on spawn.
			if (projectile.ai[0] == 0)
			{
				projectile.ai[0] = 1;
				projectile.localAI[0] = Main.rand.Next(90, 241);
			}

			// Spawn a projectile every random interval.
			// Try to target the NPC closest to the player.
			if (Main.myPlayer == projectile.owner)
			{
				if (projectile.localAI[0]-- <= 0)
				{
					projectile.localAI[0] = Main.rand.Next(90, 241);

					int target = 200;
					float currentRange = 600;
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						float distance = owner.Distance(Main.npc[i].Center);
						if (distance < currentRange/* && Main.npc[i].CanBeChasedBy(projectile)*/)
						{
							target = i;
							currentRange = owner.Distance(Main.npc[i].Center);
						}
					}

					if (target != Main.maxNPCs)
					{
						Vector2 newProjVelocity = Vector2.Normalize(projectile.DirectionTo(Main.npc[target].Center)) * 10;
						Projectile.NewProjectile(projectile.Center + new Vector2(0, -16), newProjVelocity, ModContent.ProjectileType<MartianEngineerSoulProj_TeslaShot>(), projectile.damage, .5f, owner.whoAmI);
					}
				}
			}

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool? CanHitNPC(NPC target) => false;
		public override bool OnTileCollide(Vector2 oldVelocity) => false;
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (base.TileCollideStyle(ref width, ref height, ref fallThrough));
		}
	}

	internal sealed class MartianEngineerSoulProj_TeslaShot : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.MartianTurretBolt;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
			DisplayName.SetDefault("Tesla Shot");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 10;

			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1] == 0f)
			{
				SpawnDust();
				projectile.ai[1] = 1f;
				SoundEngine.PlaySound(SoundID.Item12, projectile.position);
			}

			projectile.alpha -= 40;
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}

			projectile.spriteDirection = projectile.direction;
			if (++projectile.frameCounter >= 3)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.3f, 0.8f, 1.1f);

			projectile.rotation = projectile.velocity.ToRotation();
			if (projectile.direction == -1)
			{
				projectile.rotation += MathHelper.Pi;
			}
			return (false);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			lightColor = Color.Lerp(lightColor, Color.White, 0.8f);
			return new Color(lightColor.R, lightColor.G, lightColor.B, 25);
		}

		public override void Kill(int timeLeft) => SpawnDust();

		private void SpawnDust()
		{
			int randAmount = Main.rand.Next(5, 10);
			for (int i = 0; i < randAmount; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.Center, 0, 0, 226, 0f, 0f, 100, default, 0.5f)];
				newDust.velocity *= 1.6f;
				newDust.velocity.Y -= 1f;
				newDust.position = Vector2.Lerp(newDust.position, projectile.Center, 0.5f);
				newDust.noGravity = true;
			}
		}
	}
}
