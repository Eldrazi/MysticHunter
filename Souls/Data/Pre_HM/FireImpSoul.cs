﻿#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class FireImpSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.FireImp;
		public override string soulDescription => "Cast a small fireball.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(20 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 7f;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<FireImpSoulProj>(), 25 + 2*stack, .1f + .01f*stack, p.whoAmI, stack == 9 ? 1 : 0);

			return (true);
		}
	}

	public class FireImpSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BallofFire;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fireball");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.friendly = true;
			projectile.hostile = false;

			projectile.alpha = 100;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				SoundEngine.PlaySound(SoundID.Item20, projectile.position);
			}

			for (int i = 0; i < 2; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100, default, 2f)];
				d.noGravity = true;
				d.velocity *= .3f;
			}
			projectile.rotation += 0.3f * projectile.direction;

			// Explosion code.
			if (projectile.owner == Main.myPlayer && projectile.ai[0] != 0 && projectile.timeLeft <= 3)
			{
				projectile.alpha = 255;
				projectile.tileCollide = false;

				projectile.position.X = projectile.position.X + (projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (projectile.height / 2);
				projectile.width = 80;
				projectile.height = 80;
				projectile.position.X = projectile.position.X - (projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (projectile.height / 2);
			}
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.timeLeft = 3;
			projectile.velocity *= 0f;
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (oldVelocity.X != projectile.velocity.X)
				projectile.velocity.X = -oldVelocity.X;
			if (oldVelocity.Y != projectile.velocity.Y)
				projectile.velocity.Y = -oldVelocity.Y;

			SoundEngine.PlaySound(SoundID.Item10, projectile.position);
			return (false);
		}

		public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 25);

		public override void Kill(int timeLeft)
		{
			// Small explosion.
			if (projectile.ai[0] == 0)
			{
				SoundEngine.PlaySound(SoundID.Item10, projectile.position);
				for (int i = 0; i < 20; i++)
				{
					Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f)];
					d.noGravity = true;
					d.velocity *= 2f;

					d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)];
					d.velocity *= 2f;
				}
				return;
			}

			// Large explosion, no actual collision involved (happens in PreAI).
			SoundEngine.PlaySound(SoundID.Item14, projectile.position);
			projectile.position.X = projectile.position.X + (projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (projectile.height / 2);
			projectile.width = 80;
			projectile.height = 80;
			projectile.position.X = projectile.position.X - (projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (projectile.height / 2);

			for (int i = 0; i < 40; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 2f)];
				d.velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}

			for (int i = 0; i < 70; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f)];
				d.noGravity = true;
				d.velocity *= 5f;

				d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f)];
				d.velocity *= 2f;
			}
			projectile.position.X = projectile.position.X + (projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (projectile.height / 2);
			projectile.width = 10;
			projectile.height = 10;
			projectile.position.X = projectile.position.X - (projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (projectile.height / 2);
		}
	}
}
