#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

#endregion

namespace MysticHunter.Souls.Projectiles
{
	internal sealed class BloodTrail : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_0";

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 12;

			projectile.penetrate = 3;
			projectile.timeLeft = 120;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default, 1.5f)];
			newDust.noGravity = true;

			return (false);
		}
	}
}
