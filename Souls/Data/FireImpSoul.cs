using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class FireImpSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.FireImp;
		public string soulName => "Fire Imp";
		public string soulDescription => "Boosts stats while in the underworld.";

		public short cooldown => 300;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => (short)(20 + stack);
		public bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 7f;

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<FireImpSoulProj>(), 25 + 2*stack, .1f + .01f*stack, p.whoAmI, stack == 9 ? 1 : 0);

			return (true);
		}
	}

	public class FireImpSoulProj : ModProjectile
	{
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
				Main.PlaySound(SoundID.Item20, projectile.position);
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
			return (false);
		}

		public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 25);

		public override void Kill(int timeLeft)
		{
			// Small explosion.
			if (projectile.ai[0] == 0)
			{
				Main.PlaySound(SoundID.Item10, projectile.position);
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
			Main.PlaySound(SoundID.Item14, projectile.position);
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

			/*for (int i = 0; i < 3; ++i)
			{
				float num738 = 0.33f * (i+1);

				int num739 = Gore.NewGore(new Vector2(projectile.position.X + (projectile.width / 2) - 24f, projectile.position.Y + (projectile.height / 2) - 24f), default, Main.rand.Next(61, 64));
				Gore gore = Main.gore[num739];
				gore.velocity *= num738;
				Gore gore118 = Main.gore[num739];
				gore118.velocity.X = gore118.velocity.X + 1f;
				Gore gore119 = Main.gore[num739];
				gore119.velocity.Y = gore119.velocity.Y + 1f;
				num739 = Gore.NewGore(new Vector2(base.position.X + (float)(base.width / 2) - 24f, base.position.Y + (float)(base.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
				gore = Main.gore[num739];
				gore.velocity *= num738;
				Gore gore120 = Main.gore[num739];
				gore120.velocity.X = gore120.velocity.X - 1f;
				Gore gore121 = Main.gore[num739];
				gore121.velocity.Y = gore121.velocity.Y + 1f;
				num739 = Gore.NewGore(new Vector2(base.position.X + (float)(base.width / 2) - 24f, base.position.Y + (float)(base.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
				gore = Main.gore[num739];
				gore.velocity *= num738;
				Gore gore122 = Main.gore[num739];
				gore122.velocity.X = gore122.velocity.X + 1f;
				Gore gore123 = Main.gore[num739];
				gore123.velocity.Y = gore123.velocity.Y - 1f;
				num739 = Gore.NewGore(new Vector2(base.position.X + (float)(base.width / 2) - 24f, base.position.Y + (float)(base.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64));
				gore = Main.gore[num739];
				gore.velocity *= num738;
				Gore gore124 = Main.gore[num739];
				gore124.velocity.X = gore124.velocity.X - 1f;
				Gore gore125 = Main.gore[num739];
				gore125.velocity.Y = gore125.velocity.Y - 1f;
			}*/
			projectile.position.X = projectile.position.X + (projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (projectile.height / 2);
			projectile.width = 10;
			projectile.height = 10;
			projectile.position.X = projectile.position.X - (projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (projectile.height / 2);
		}
	}
}
