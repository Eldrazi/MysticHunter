using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class IchorStickerSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.IchorSticker;
		public override string soulDescription => "Fires a spray of ichor.";

		public override short cooldown => 8;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 3;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 12.5f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<IchorStickerSoulProj>(), 15 + 3 * stack, .2f, p.whoAmI);
			return (true);
		}
	}

	public class IchorStickerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_22";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ichor Spray");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;

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
					Dust d = Main.dust[Dust.NewDust(projectile.position + Vector2.One * size, projectile.width - size * 2, projectile.height - size * 2, 170, 0f, 0f, 100, default, 1.2f)];
					d.noGravity = true;
					d.velocity *= .15f;
					d.position.X -= - num169;
					d.position.Y -= num170;
				}

				if (Main.rand.Next(8) == 0)
				{
					Dust d = Main.dust[Dust.NewDust(projectile.position + Vector2.One * size, projectile.width - size * 2, projectile.height - size * 2, 170, 0f, 0f, 100, default, .75f)];
					d.velocity *= 0.75f;

				}
			}
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.Ichor, 90);
		}
	}
}
