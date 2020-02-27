using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class RedSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.RedSlime;
		public string soulDescription => "Fires a spray of red slime.";

		public short cooldown => 8;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => 1;
		public bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 12.5f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<RedSlimeSoulProj>(), 5 + stack, 5, p.whoAmI);
			return (true);
		}
	}

	public class RedSlimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_22";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slime");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.extraUpdates = 2;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			projectile.scale -= .02f;
			if (projectile.scale <= 0)
				projectile.Kill();

			if (projectile.ai[0]++ > 1)
			{
				projectile.velocity.Y += .2f;

				int size = 6;

				for (int i = 0; i < 3; ++i)
				{
					float num169 = projectile.velocity.X / 3f * i;
					float num170 = projectile.velocity.Y / 3f * i;
					Dust d = Main.dust[Dust.NewDust(projectile.position + Vector2.One * size, projectile.width - size * 2, projectile.height - size * 2, DustID.SomethingRed, 0f, 0f, 100, default, 1.2f)];
					d.noGravity = true;
					d.velocity *= .15f;
					d.position.X -= - num169;
					d.position.Y -= num170;
				}

				if (Main.rand.Next(8) == 0)
				{
					Dust d = Main.dust[Dust.NewDust(projectile.position + Vector2.One * size, projectile.width - size * 2, projectile.height - size * 2, DustID.SomethingRed, 0f, 0f, 100, default, .75f)];
					d.velocity *= 0.75f;

				}
			}
			/*
				scale -= 0.02f;
				if (scale <= 0f)
				{
					Kill();
				}
				if (this.ai[0] > 3f)
				{
					base.velocity.Y = base.velocity.Y + 0.2f;
					int num4;
					for (int num167 = 0; num167 < 1; num167 = num4 + 1)
					{
						for (int num168 = 0; num168 < 3; num168 = num4 + 1)
						{
							float num169 = base.velocity.X / 3f * (float)num168;
							float num170 = base.velocity.Y / 3f * (float)num168;
							int num171 = 6;
							int num172 = Dust.NewDust(new Vector2(base.position.X + (float)num171, base.position.Y + (float)num171), width - num171 * 2, height - num171 * 2, 172, 0f, 0f, 100, default(Color), 1.2f);
							Main.dust[num172].noGravity = true;
							Dust dust3 = Main.dust[num172];
							dust3.velocity *= 0.3f;
							dust3 = Main.dust[num172];
							dust3.velocity += base.velocity * 0.5f;
							Dust dust22 = Main.dust[num172];
							dust22.position.X = dust22.position.X - num169;
							Dust dust23 = Main.dust[num172];
							dust23.position.Y = dust23.position.Y - num170;
							num4 = num168;
						}
						if (Main.rand.Next(8) == 0)
						{
							int num173 = 6;
							int num174 = Dust.NewDust(new Vector2(base.position.X + (float)num173, base.position.Y + (float)num173), width - num173 * 2, height - num173 * 2, 172, 0f, 0f, 100, default(Color), 0.75f);
							Dust dust3 = Main.dust[num174];
							dust3.velocity *= 0.5f;
							dust3 = Main.dust[num174];
							dust3.velocity += base.velocity * 0.5f;
						}
						num4 = num167;
					}
				}
				else
				{
					ref float reference = ref this.ai[0];
					reference += 1f;
				}*/
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.Slow, 60);
		}
	}
}
