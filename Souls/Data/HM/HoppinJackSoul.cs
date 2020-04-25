using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class HoppinJackSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.HoppinJack;
		public override string soulDescription => "Throw an exploding pumpkin.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 85 + 5 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<HoppinJackSoulProj>(), damage, .25f, p.whoAmI, stack);

			return (true);
		}
	}

	public class HoppinJackSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Item_1725";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pumpkin");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
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
				projectile.velocity.Y += .2f;
				if (projectile.velocity.Y > 8)
					projectile.velocity.Y = 8;

				projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * projectile.direction * .05f;
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
			Main.PlaySound(SoundID.Item14, projectile.position);

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
