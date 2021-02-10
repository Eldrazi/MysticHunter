#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class ButcherSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Butcher;
		public override string soulDescription => "Attack with a chainsaw.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 120;

			if (stack >= 5)
			{
				damage += 50;
			}
			if (stack >= 9)
			{
				damage += 50;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<ButcherSoul_Proj>(), damage, 2f, p.whoAmI, 0);

			return (true);
		}
	}

	public class ButcherSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.ButchersChainsaw;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chainsaw");
			Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.ButchersChainsaw];
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = 3;
			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			if (++projectile.ai[0] >= 15)
			{
				projectile.velocity.X *= 0.98f;
				projectile.velocity.Y += 0.2f;
			}

			projectile.spriteDirection = -projectile.direction;
			projectile.rotation += projectile.velocity.X * 0.05f;

			if (projectile.frameCounter++ >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor * projectile.Opacity);
	}
}
