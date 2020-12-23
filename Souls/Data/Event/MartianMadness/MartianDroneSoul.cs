using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class MartianDroneSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MartianDrone;
		public override string soulDescription => "Summon a homing Martian Drone.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 110 + 10 * stack;

			Vector2 projVel = Vector2.Normalize(Main.MouseWorld - p.Center) * 4;
			Projectile.NewProjectile(p.Center, projVel, ProjectileType<MartianDroneSoulProj>(), damage, .5f, p.whoAmI);

			return (true);
		}
	}

	internal sealed class MartianDroneSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.MartianDrone;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
			DisplayName.SetDefault("Martian Drone");
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 18;
			
			projectile.scale = .8f;
			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;

			drawOffsetX = -20;
			drawOriginOffsetY = -12;
		}

		public override bool PreAI()
		{
			float velocityLength = projectile.velocity.Length();

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = velocityLength;
			}

			// Homing state.
			if (projectile.ai[0] == 0)
			{
				if (projectile.alpha > 0)
					projectile.alpha -= 25;
				else
					projectile.alpha = 0;

				int target = 0;
				bool hasTarget = false;
				float maxHomingRange = 300;
				float xPos = projectile.position.X;
				float yPos = projectile.position.Y;

				// If the projectile doesn't have a target, search for one.
				if (projectile.ai[1] == 0f)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						if (Main.npc[i].CanBeChasedBy(projectile) && (projectile.ai[1] == 0 || projectile.ai[1] == i + 1))
						{
							float targetX = Main.npc[i].position.X + (Main.npc[i].width / 2);
							float targetY = Main.npc[i].position.Y + (Main.npc[i].height / 2);
							float targetDistance = projectile.Distance(Main.npc[i].Center);

							if (targetDistance < maxHomingRange && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
							{
								xPos = targetX;
								yPos = targetY;
								maxHomingRange = targetDistance;
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

					if (npc.active && npc.CanBeChasedBy(projectile, true) && !npc.dontTakeDamage)
					{
						float targetX = npc.Center.X;
						float targetY = npc.Center.Y;
						float targetDistance = projectile.Distance(npc.Center);
						if (targetDistance < 1000f)
						{
							hasTarget = true;
							xPos = npc.Center.X;
							yPos = npc.Center.Y;
						}
					}
					else
					{
						projectile.ai[1] = 0;
					}
				}

				// Homing behaviour.
				if (hasTarget)
				{
					Vector2 targetVelocity = Vector2.Normalize(new Vector2(xPos, yPos) - projectile.Center) * projectile.localAI[0];

					float homingSpeed = 8;
					projectile.velocity.X = (projectile.velocity.X * (homingSpeed - 1) + targetVelocity.X) / homingSpeed;
					projectile.velocity.Y = (projectile.velocity.Y * (homingSpeed - 1) + targetVelocity.Y) / homingSpeed;
				}

				// Graphical/animation.
				if (projectile.frameCounter++ >= 10)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				}

				projectile.spriteDirection = projectile.direction;
				projectile.rotation = projectile.velocity.ToRotation() + (projectile.direction == -1 ? MathHelper.Pi : 0);

				if (projectile.timeLeft <= 3)
					projectile.ai[0] = 1;
			}
			// Explosion state.
			else if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.tileCollide = false;
				projectile.ai[1] = 0f;
				projectile.alpha = 255;

				projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
				projectile.width = 128;
				projectile.height = 128;
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				projectile.knockBack = 8f;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.velocity *= 0f;
			projectile.alpha = 255;
			projectile.timeLeft = 3;
			projectile.ai[0] = 1;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity *= 0f;
			projectile.alpha = 255;
			projectile.timeLeft = 3;
			projectile.ai[0] = 1;
			return (true);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item14, projectile.position);

			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 22;
			projectile.height = 22;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);

			for (int i = 0; i < 30; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 1.4f;
			}

			for (int i = 0; i < 20; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f)];
				newDust.velocity *= 7f;
				newDust.noGravity = true;

				newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 3f;
			}
		}
	}
}
