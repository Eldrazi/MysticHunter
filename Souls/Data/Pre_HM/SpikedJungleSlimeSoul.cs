﻿using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SpikedJungleSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.SpikedJungleSlime;
		public string soulDescription => "Fires poisonous spikes.";

		public short cooldown => 180;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => 5;
		public bool SoulUpdate(Player p, short stack)
		{
			int amount = 2 + stack;

			for (int i = 0; i < amount; ++i)
			{
				float rotValue = (float)(Math.PI / (amount - 1)) * i;

				Vector2 v2 = new Vector2((float)Math.Cos(rotValue), (float)Math.Sin(-rotValue)) * 7f;
				Projectile.NewProjectile(p.Center, v2, ProjectileType<SpikedJungleSlimeSoulProj>(), 5, .1f, p.whoAmI);
			}
			Main.PlaySound(SoundID.Item17, p.Center);
			return (true);
		}
	}

	public class SpikedJungleSlimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_176";

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 4;

			projectile.hostile = false;
			projectile.friendly = true;

			projectile.timeLeft = 50;
		}

		public override bool PreAI()
		{
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.Poisoned, 180);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 8; ++i)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GrassBlades, projectile.velocity.X, projectile.velocity.Y);
		}
	}
}
