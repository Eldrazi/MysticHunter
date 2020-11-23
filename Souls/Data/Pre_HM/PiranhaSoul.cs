using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class PiranhaSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Piranha;
		public override string soulDescription => "Summons a latching piranha.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(8 + 2 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int latchTime = 240 + (60 * stack);

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<PiranhaSoulProj>(), 5 + stack, .1f, p.whoAmI, 0, latchTime);

			// Play 'minion summon' item sound.
			Main.PlaySound(SoundID.Item44, p.position);
			return (true);
		}
	}

	public class PiranhaSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_190";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Piranha");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.alpha = 255;
			projectile.penetrate = -1;
			
			projectile.magic = true;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			projectile.alpha -= 50;
			if (projectile.alpha < 0)
				projectile.alpha = 0;

			// If the projectile has collided with an NPC and thus has a target.
			if (projectile.ai[0] > 0)
			{
				projectile.tileCollide = false;

				NPC target = Main.npc[(int)projectile.ai[0]];

				if (target.active && target.life > 0)
				{
					float minDist = 16f;

					Vector2 targetDir = new Vector2(
						target.position.X + (target.width / 2) - projectile.Center.X,
						target.position.Y + (target.height / 2) - projectile.Center.Y);

					bool changeRot = true;
					float len = targetDir.Length();
					if (len < minDist)
					{
						projectile.velocity = targetDir;
						if (len <= minDist / 2f)
							changeRot = false;
					}
					else
					{
						len = minDist / len;
						targetDir *= len;
						projectile.velocity = targetDir;
					}

					if (changeRot)
					{
						if (projectile.velocity.X < 0f)
						{
							projectile.spriteDirection = -1;
							projectile.rotation = (float)Math.Atan2(-projectile.velocity.Y, -projectile.velocity.X);
						}
						else
						{
							projectile.spriteDirection = 1;
							projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
						}
					}
				}
				else
					projectile.Kill();
			}
			else
			{
				if (projectile.velocity.X < 0f)
				{
					projectile.spriteDirection = -1;
					projectile.rotation = (float)Math.Atan2(-projectile.velocity.Y, -projectile.velocity.X);
				}
				else
				{
					projectile.spriteDirection = 1;
					projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
				}
			}

			// Animate the projectile.
			if (projectile.frameCounter++ >= 4)
			{
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				projectile.frameCounter = 0;
			}
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.ai[0] == 0f)
			{
				projectile.ai[0] = target.whoAmI;
				projectile.timeLeft = (int)projectile.ai[1];
				projectile.netUpdate = true;
			}
			if (Main.player[projectile.owner].Center.X < projectile.Center.X)
				projectile.direction = 1;
			else
				projectile.direction = -1;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GreenBlood, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
