#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class IcyMermanSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.IcyMerman;
		public override string soulDescription => "Shoot frosty icicles.";

		public override short cooldown => 10;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30 + (5 * stack);

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center).RotatedByRandom(.35f) * 8f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<IcyMermanSoulProj>(), damage, .2f, p.whoAmI, stack);

			return (true);
		}
	}

	public class IcyMermanSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.IceSpike;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ice Spike");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 6;

			projectile.alpha = 255;
			projectile.penetrate = 2;

			projectile.friendly = true;
			projectile.coldDamage = true;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			int maxTimeleft = 10 + (int)(10 * projectile.ai[0]);
			if (projectile.timeLeft > maxTimeleft)
				projectile.timeLeft = maxTimeleft;

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (projectile.alpha == 0)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.oldPosition - projectile.velocity * 3f, projectile.width, projectile.height, 76, 0f, 0f, 50)];
				d.noGravity = true;
				d.noLight = true;
				d.velocity *= 0.5f;
			}
			projectile.alpha -= 50;
			if (projectile.alpha < 0)
				projectile.alpha = 0;

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				SoundEngine.PlaySound(SoundID.Item17, projectile.position);
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Ice, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
