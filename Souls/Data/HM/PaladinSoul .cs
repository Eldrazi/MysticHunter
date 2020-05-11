using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class PaladinSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Paladin;
		public override string soulDescription => "Throw an arching hammer.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 120 + 10 * stack;
			int amount = 1 + (int)(stack / 3);

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 10f;
			for (int i = 0; i < amount; ++i)
				Projectile.NewProjectile(p.Center, velocity.RotatedByRandom(.3f), ProjectileType<PaladinSoulProj>(), damage, 1, p.whoAmI, stack);

			return (true);
		}
	}

	public class PaladinSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_300";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Paladin Hammer");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;

			projectile.scale = .8f;
			projectile.penetrate = -1;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0]++ >= 30)
			{
				projectile.velocity.Y += .08f;
				if (projectile.velocity.Y > 8)
					projectile.velocity.Y = 8;

				projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * .05f * projectile.direction;
			}
			else
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			projectile.velocity.X *= .99f;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, .8f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
