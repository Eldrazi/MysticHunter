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
	public class GastropodSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Gastropod;
		public override string soulDescription => "Fires a precise laser.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 50 + 3 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<GastropodSoulProj>(), damage, .2f, p.whoAmI, stack + 1);

			return (true);
		}
	}

	public class GastropodSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PinkLaser;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 4;

			projectile.alpha = 255;
			projectile.scale = 1.2f;
			projectile.light = .75f;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 2;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			projectile.alpha -= 15;
			if (projectile.alpha < 0)
				projectile.alpha = 0;

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 0;
				SoundEngine.PlaySound(SoundID.Item12, projectile.position);
			}

			if (projectile.maxPenetrate < projectile.ai[0])
				projectile.penetrate = projectile.maxPenetrate = (int)projectile.ai[0];
			return (true);
		}

		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, projectile.position);
		}
	}
}
