using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class BloodFeederSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.BloodFeeder;
		public override string soulDescription => "Throw an confusing Blood Feeder.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 25 + 2 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<BloodFeederSoulProj>(), damage, .25f, p.whoAmI, stack);

			return (true);
		}
	}

	public class BloodFeederSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_241";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Feeder");
			Main.projFrames[projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = 1;

			projectile.friendly = true;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			projectile.velocity.Y += .2f;
			if (projectile.velocity.Y > 8)
				projectile.velocity.Y = 8;

			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * projectile.direction * .05f;

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int randMax = (int)(11 - projectile.ai[0]);
			if (Main.rand.Next(randMax) == 0)
				target.AddBuff(BuffID.Confused, 300);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
