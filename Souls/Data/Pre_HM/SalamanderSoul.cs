using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SalamanderSoul : BaseSoul
	{
		public override short soulNPC => NPCID.Salamander;
		public override string soulDescription => "Spit a random debuff ball.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 6;

		private readonly int[] randomBuffs = new int[] {
			BuffID.Poisoned, BuffID.OnFire, BuffID.Oiled, BuffID.Midas, BuffID.Wet, // First stage debuffs.
			BuffID.Ichor, BuffID.Venom, BuffID.CursedInferno, // Second stage debuffs.
			BuffID.ShadowFlame, BuffID.Confused, BuffID.Frostburn // Third stage debuffs.
		};
		public override bool SoulUpdate(Player p, short stack)
		{
			int randBuffMax = 5;
			if (stack >= 5)
				randBuffMax += 3;
			if (stack >= 9)
				randBuffMax += 3;

			float debuffTime = 120 + (30 * stack);

			Vector2 spawnPos = p.Center - new Vector2(0, 12 * p.gravDir);
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - spawnPos) * 6;
			Projectile.NewProjectile(spawnPos, velocity, ProjectileType<SalamanderSoulProj>(), 4 + stack, .1f, p.whoAmI, randomBuffs[Main.rand.Next(randBuffMax)], debuffTime);
			return (true);
		}
	}

	public class SalamanderSoulProj : ModProjectile
	{
		// projectile.ai[0] = debuff type.
		// projectile.ai[1] = debuff time.

		public override string Texture => "Terraria/Projectile_572";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spit");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 10;

			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			projectile.alpha -= 20;
			if (projectile.alpha < 0)
				projectile.alpha = 0;

			// Play a sound at spawn of the projectile.
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlaySound(SoundID.Item17, projectile.position);
			}

			// Spawn dusts (which make up the visual aspect of the projectile).
			for (int i = 0; i < 2; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 40, projectile.velocity.X * .5f, projectile.velocity.Y * .5f, 100)];
				d.velocity += projectile.velocity;
				d.velocity *= 0.5f;
				d.noGravity = true;
				d.scale = 1.2f;
				d.position = (projectile.Center + projectile.position) / 2f;
			}

			// Add gravitational pull.
			projectile.velocity.Y += .04f;
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff((int)projectile.ai[0], (int)projectile.ai[1]);
		}
	}
}
