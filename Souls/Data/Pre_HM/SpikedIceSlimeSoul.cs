using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SpikedIceSlimeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.SpikedIceSlime;
		public override string soulDescription => "Fires frosty spikes.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 2 + stack;

			for (int i = 0; i < amount; ++i)
			{
				float rotValue = (float)(Math.PI / (amount - 1)) * i;

				Vector2 v2 = new Vector2((float)Math.Cos(rotValue), (float)Math.Sin(-rotValue)) * 7f;
				Projectile.NewProjectile(p.Center, v2, ProjectileType<SpikedIceSlimeSoulProj>(), 5, .1f, p.whoAmI);
			}
			Main.PlaySound(SoundID.Item17, p.Center);
			return (true);
		}
	}

	public class SpikedIceSlimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_174";

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
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice, projectile.velocity.X, projectile.velocity.Y);
		}
	}
}
