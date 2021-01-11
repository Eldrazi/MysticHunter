#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class ArapaimaSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Arapaima;
		public override string soulDescription => "Fire an Arapaima torpedo.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30 + 2 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 5f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<ArapaimaSoulProj>(), damage, 1, p.whoAmI, stack);

			return (true);
		}
	}

	public class ArapaimaSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arapaima Torpedo");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 36;

			projectile.scale = .8f;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}

		public override bool PreAI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + (float)System.Math.PI;

			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.alpha = 255;
				projectile.tileCollide = false;

				projectile.position.X = projectile.position.X + (projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (projectile.height / 2);
				projectile.width = 128;
				projectile.height = 128;
				projectile.position.X = projectile.position.X - (projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (projectile.height / 2);
				projectile.knockBack = 8f;
			}
			else
			{
				float maxSpeed = 9;
				if (projectile.wet)
					maxSpeed = 15;

				if (System.Math.Abs(projectile.velocity.X) >= 8f || System.Math.Abs(projectile.velocity.Y) >= 8f)
				{
					for (int i = 0; i < 2; ++i)
					{
						float num256 = 0f;
						float num257 = 0f;
						if (i == 1)
						{
							num256 = projectile.velocity.X * 0.5f;
							num257 = projectile.velocity.Y * 0.5f;
						}
						Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X + 3f + num256, projectile.position.Y + 3f + num257) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100)];
						d.scale *= 2f + Main.rand.Next(10) * 0.1f;
						d.velocity *= 0.2f;
						d.noGravity = true;

						d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X + 3f + num256, projectile.position.Y + 3f + num257) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default, .5f)];
						d.fadeIn = 1f + Main.rand.Next(5) * 0.1f;
						d.velocity *= 0.05f;
					}
				}
				if (System.Math.Abs(projectile.velocity.X) < maxSpeed && System.Math.Abs(projectile.velocity.Y) < maxSpeed)
					projectile.velocity *= 1.1f;

				projectile.spriteDirection = -projectile.direction;

				projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.velocity *= 0f;
			projectile.alpha = 255;
			projectile.timeLeft = 3;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity *= 0f;
			projectile.alpha = 255;
			projectile.timeLeft = 3;
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, projectile.position);

			projectile.position.X = projectile.position.X + (projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (projectile.height / 2);
			projectile.width = 2;
			projectile.height = 2;
			projectile.position.X = projectile.position.X - (projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (projectile.height / 2);

			for (int i = 0; i < 30; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f)];
				d.velocity *= 1.4f;
			}

			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f)];
				d.noGravity = true;
				d.velocity *= 7f;
				d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f)];
				d.velocity *= 3f;
			}

			for (int i = 0; i < 2; ++i)
			{
				float scale = .4f;
				if (i == 1)
					scale += .4f;

				Gore g = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				g.velocity = g.velocity * scale + Vector2.One;

				g = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				g.velocity = g.velocity * scale + new Vector2(-1, 1);

				g = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				g.velocity = g.velocity * scale + new Vector2(1, -1);

				g = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				g.velocity = g.velocity * scale + new Vector2(-1, -1);
			}
		}
	}
}
