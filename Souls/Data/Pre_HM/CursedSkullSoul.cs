using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class CursedSkullSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.CursedSkull;
		public override string soulDescription => "Fires a homing skull.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Calculate the required velocity of the bees towards the cursor.
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 5f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<CursedSkullSoulProj>(), 10 + 2 * stack, .1f, p.whoAmI, 20 - stack);
			return (true);
		}
	}

	public class CursedSkullSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_34";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;

			projectile.alpha = 255;

			drawOffsetX = -8;
			drawOriginOffsetY = -6;
		}

		public override bool PreAI()
		{
			if (projectile.alpha > 0)
				projectile.alpha -= 20;
			else
				projectile.alpha = 0;

			// Cache the start speed.
			if (projectile.localAI[0] == 0)
			{
				Main.PlaySound(SoundID.NPCDeath2, projectile.Center);
				projectile.localAI[0] = projectile.velocity.Length();
			}

			int targetIndex = 0;
			float maxRange = 300f;
			bool targetAcquired = false;

			float distanceToTarget = 0;
			Vector2 targetPos = projectile.position;

			// Target acquisition.
			if (projectile.ai[1] == 0)
			{

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC target = Main.npc[i];
					if (target.CanBeChasedBy(projectile))
					{
						Vector2 targetDir = target.Center - projectile.Center;
						float targetDirLength = targetDir.Length();

						if (targetDirLength < maxRange)
						{
							targetIndex = i;
							targetAcquired = true;
							maxRange = targetDirLength;
						}
					}
				}
				if (targetAcquired)
					projectile.ai[1] = targetIndex;
				targetAcquired = false;
			}

			// Target check.
			if (projectile.ai[1] > 0)
			{
				NPC target = Main.npc[(int)projectile.ai[1]];

				if (target.active && target.CanBeChasedBy(projectile, true) && target.dontTakeDamage == false)
				{
					targetPos = target.Center;
					distanceToTarget = (targetPos - projectile.Center).Length();
					if (distanceToTarget < 1000f)
						targetAcquired = true;
				}
				else
					projectile.ai[1] = 0;
			}

			// Chasing behavior.
			if (targetAcquired)
			{
				targetPos -= projectile.Center;
				distanceToTarget = projectile.localAI[0] / distanceToTarget;
				targetPos *= distanceToTarget;

				projectile.velocity = new Vector2(
					(projectile.velocity.X * (projectile.ai[0] - 1) + targetPos.X) / projectile.ai[0], 
					(projectile.velocity.Y * (projectile.ai[0] - 1) + targetPos.Y) / projectile.ai[0]);
			}

			// Visuals.
			projectile.spriteDirection = projectile.direction;
			projectile.rotation = projectile.velocity.ToRotation() - (projectile.direction == -1 ? (float)Math.PI : 0);

			if (projectile.frameCounter++ >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.AncientLight, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
