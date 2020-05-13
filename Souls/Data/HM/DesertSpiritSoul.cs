using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class DesertSpiritSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DesertDjinn;
		public override string soulDescription => "Summon a wave of dark fire.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			float maxRadius = 120 + 30 * stack;

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<DesertSpiritSoulProj>(), 50 + 5 * stack, .1f, p.whoAmI, maxRadius);
			return (true);
		}
	}


	public class DesertSpiritSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_0";

		private float MaxDamageRadius { get { return projectile.ai[0]; } }
		private float CurrentDamageRadius { get { return projectile.ai[1]; } set { projectile.ai[1] = value; } }

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fire Wave");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 1;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (CurrentDamageRadius == 0)
				Main.PlaySound(SoundID.Item20, projectile.position);

			CurrentDamageRadius += (MaxDamageRadius / 30);
			if (CurrentDamageRadius >= MaxDamageRadius)
				projectile.Kill();

			for (float i = 0; i < 3 * Math.PI; i += (float)Math.PI / (projectile.ai[1] * .1f))
			{
				Vector2 spawnPos = projectile.Center + new Vector2((float)Math.Cos(i), (float)Math.Sin(i)) * CurrentDamageRadius;
				Vector2 velocity = Vector2.Normalize(spawnPos - projectile.Center);

				if (Collision.SolidCollision(spawnPos, 2, 2))
					continue;

				Dust d = Main.dust[Dust.NewDust(spawnPos, 2, 2, 27, velocity.X, velocity.Y, 100, default, 2)];
				d.noGravity = true;
			}

			return (false);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 center = new Vector2(targetHitbox.X + targetHitbox.Width * .5f, targetHitbox.Y + targetHitbox.Height * .5f);

			if (Vector2.Distance(projectile.Center, center) <= CurrentDamageRadius)
				return (true);
			return (false);
		}
	}
}
