﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data
{
	public class DarkCasterSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DarkCaster;
		public string soulName => "Dark Caster";
		public string soulDescription => "Summons a bolt of cursed water.";

		public short cooldown => 300;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => (short)(15 + stack);
		public bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center);
			velocity *= 6;

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<DarkCasterSoulProj>(), 18 + 2 * stack, .1f + .01f * stack, p.whoAmI);
			return (true);
		}
	}

	public class DarkCasterSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Water");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;
			projectile.alpha = 255;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			// Spawn a dust trail.
			for (int i = 0; i < 2; i++)
			{
				for (int j = i; j < 3; j++)
				{
					float xMod = projectile.velocity.X / 3f * i;
					float yMod = projectile.velocity.Y / 3f * i;

					Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X + 2, projectile.position.Y + 2), projectile.width - 4, projectile.height - 2 * 2, 172, 0f, 0f, 100, default, 1.2f)];
					d.noGravity = true;
					d.velocity = (d.velocity * .1f) + projectile.velocity * .5f;
					d.position.X -= xMod;
					d.position.Y -= yMod;
				}
				if (Main.rand.Next(5) == 0)
				{
					Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X + 2, projectile.position.Y + 2), projectile.width - 4, projectile.height - 2 * 2, 172, 0f, 0f, 100, default, .6f)];
					d.velocity = (d.velocity * .25f) + projectile.velocity * .5f;
				}
			}
			projectile.rotation += 0.4f * projectile.direction;
			return (false);
		}
	}
}
