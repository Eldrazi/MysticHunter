#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class BlackRecluseSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.BlackRecluse;
		public override string soulDescription => "Fires a spread of poisonous fangs.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 20 + 5 * stack;

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

				Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<BlackRecluseSoulProj>(), damage, .2f, p.whoAmI, Math.Abs(6 * (i - delayHelper)));
			}
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.BlackRecluseWall };
	}

	public class BlackRecluseSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fang");
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

			if (projectile.ai[1] >= projectile.ai[0])
			{
				if (projectile.alpha > 0)
					projectile.alpha -= 20;
				else
					projectile.alpha = 0;
			}
			else // Used so the projectile doesn't move forward when it's invisible.
				projectile.position -= projectile.velocity;

			projectile.ai[1]++;
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Venom, 120);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GreenBlood, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
