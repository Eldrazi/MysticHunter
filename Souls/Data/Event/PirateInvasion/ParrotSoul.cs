#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.PirateInvasion
{
	public class ParrotSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Parrot;
		public override string soulDescription => "Fires a spread of golden feathers.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(15 + (stack >= 5 ? 5 : 0) + (stack >= 9 ? 5 : 0));
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 55;

			int amount = 3;
			int delayHelper = 1;
			if (stack >= 5)
			{
				damage += 5;
				amount += 2;
				delayHelper++;
			}
			if (stack >= 9)
			{
				damage += 5;
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

				Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<ParrotSoulProj>(), damage, .2f, p.whoAmI, Math.Abs(6 * (i - delayHelper)));
			}
			return (true);
		}
	}

	public class ParrotSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Golden Feather");
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
					SoundEngine.PlaySound(SoundID.Item17, projectile.position);

				if (projectile.alpha > 0)
					projectile.alpha -= 20;
				else
					projectile.alpha = 0;
			}
			else // Used so the projectile doesn't move forward when it's invisible.
				projectile.position -= projectile.velocity;

			Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Gold)].noGravity = true;
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Midas, 60);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Gold, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)].noLight = true;
		}
	}
}
