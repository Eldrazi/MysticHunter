#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.Rain
{
	public class FlyingFishSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.FlyingFish;
		public override string soulDescription => "Summon a homing Flying Fish.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 15 + 3 * stack;
			float homingModifier = 2.15f - .15f * stack;

			Vector2 projVel = Vector2.Normalize(Main.MouseWorld - p.Center) * 6;

			Projectile.NewProjectile(p.Center, projVel, ModContent.ProjectileType<FlyingFishSoulProj>(), damage, .5f, p.whoAmI, homingModifier);

			return (true);
		}
	}

	internal sealed class FlyingFishSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.FlyingFish;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
			DisplayName.SetDefault("Flying Fish");
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 18;
			
			projectile.alpha = 255;
			projectile.penetrate = 1;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			float velocityLength = projectile.velocity.Length();

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = velocityLength;
			}

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

				float homingSpeed = 8 * projectile.ai[0];
				projectile.velocity.X = (projectile.velocity.X * (homingSpeed - 1) + targetVelocity.X) / homingSpeed;
				projectile.velocity.Y = (projectile.velocity.Y * (homingSpeed - 1) + targetVelocity.Y) / homingSpeed;
			}

			// Graphical/animation.
			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			projectile.spriteDirection = -projectile.direction;
			projectile.rotation = projectile.velocity.ToRotation() + (projectile.direction == -1 ? MathHelper.Pi : 0);

			return (false);
		}
	}
}
