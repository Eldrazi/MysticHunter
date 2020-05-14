using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class HarpySoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Harpy;
		public override string soulDescription => "Fires a spread of feathers.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(10 + (stack >= 5 ? 5 : 0) + (stack >= 9 ? 5 : 0));
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 25;
			if (stack > 1)
				damage += stack + 1;

			int amount = 3;
			int delayHelper = 1;
			if (stack >= 5)
			{
				amount += 2;
				delayHelper++;
			}
			if (stack >= 9)
			{
				amount += 2;
				delayHelper++;
			}

			for (int i = 0; i < amount; ++i)
			{
				// Calculate the straight line vector from the player to the cursor.
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center);
				velocity *= 8;

				// Rotate the target velocity by a set degree.
				velocity = velocity.RotatedBy(MathHelper.ToRadians(-5 * (amount - 1) + (10 * i)));

				Projectile.NewProjectile(p.Center, velocity, ProjectileType<HarpySoulProj>(), damage, .2f, p.whoAmI, Math.Abs(6 * (i - delayHelper)));
			}
			return (true);
		}
	}

	public class HarpySoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Feather");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;

			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

			if (projectile.ai[1]++ >= projectile.ai[0])
			{
				if (projectile.ai[1] == projectile.ai[0])
					Main.PlaySound(SoundID.Item17, projectile.position);

				if (projectile.alpha > 0)
					projectile.alpha -= 20;
				else
					projectile.alpha = 0;
			}
			else // Used so the projectile doesn't move forward when it's invisible.
				projectile.position -= projectile.velocity;
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)].noLight = true;
		}
	}
}
