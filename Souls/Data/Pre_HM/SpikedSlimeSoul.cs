#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SpikedSlimeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.SlimeSpiked;
		public override string soulDescription => "Fires spikes.";

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
				Projectile.NewProjectile(p.Center, v2, ModContent.ProjectileType<SpikedSlimeSoulProj>(), 5, .1f, p.whoAmI);
			}
			SoundEngine.PlaySound(SoundID.Item17, p.Center);
			return (true);
		}
	}

	public class SpikedSlimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SpikedSlimeSpike;

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

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 180, Color.AliceBlue);
		}
	}
}
