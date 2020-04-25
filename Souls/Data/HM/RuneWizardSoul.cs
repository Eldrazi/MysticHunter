using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class RuneWizardSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.RuneWizard;
		public override string soulDescription => "Fire a powerful magical blast.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 80 + (5 * stack);

			if (stack >= 3)
				damage += 10;
			if (stack >= 5)
				damage += 10;
			if (stack >= 9)
				damage += 10;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<RuneWizardSoulProj>(), damage, .2f, p.whoAmI, stack);

			return (true);
		}
	}

	public class RuneWizardSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_129";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magical Blast");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 24;

			projectile.alpha = 255;
			projectile.penetrate = 3;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[1] == 0f)
			{
				projectile.localAI[1] = 1f;
				Main.PlaySound(SoundID.Item28, projectile.position);
			}

			for (int i = 0; i < 8; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 106, projectile.velocity.X, projectile.velocity.Y, 100)];
				d.noGravity = true;
				d.scale *= 1f + Main.rand.Next(5) * 0.1f;
				d.velocity *= 0.1f + Main.rand.Next(4) * 0.1f;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.CursedInferno, 180);

			if (projectile.ai[0] >= 3)
				target.AddBuff(BuffID.OnFire, 180);
			if (projectile.ai[0] >= 3)
				target.AddBuff(BuffID.Frostburn, 180);
			if (projectile.ai[0] >= 3)
				target.AddBuff(BuffID.ShadowFlame, 180);
		}
	}
}
