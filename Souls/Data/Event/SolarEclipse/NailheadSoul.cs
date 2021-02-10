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
	public class NailheadSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Nailhead;
		public override string soulDescription => "Toss a Fritz at your enemies.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 80 + 5 * stack;
			int amount = 3 + (stack / 3);

			for (int i = 0; i < amount; ++i)
			{
				int timeModifier = i * 4;
				Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<NailheadSoul_Proj>(), damage, 0.1f, p.whoAmI, 0, timeModifier);
			}

			return (true);
		}
	}

	public class NailheadSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.NailFriendly;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nail");
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
			if (projectile.ai[0] == 0)
			{
				if (++projectile.localAI[0] >= projectile.ai[1] && Main.myPlayer == projectile.owner)
				{
					projectile.velocity = Vector2.Normalize(Main.MouseWorld - Main.player[projectile.owner].Center).RotatedByRandom(MathHelper.PiOver4 / 2) * 10f;

					projectile.ai[0] = 1;
					projectile.ai[1] = 0;
					projectile.localAI[0] = 0;
					projectile.netUpdate = true;
				}

				projectile.alpha = 255;
				projectile.Center = Main.player[projectile.owner].Center;
			}
			else
			{
				projectile.alpha -= 25;
				if (projectile.alpha < 0)
					projectile.alpha = 0;

				if (projectile.ai[1] == 0)
				{
					Main.PlaySound(SoundID.Item17, projectile.position);
				}
				if (++projectile.ai[1] >= 20)
				{
					projectile.velocity.X *= 0.98f;
					projectile.velocity.Y += 0.2f;
				}
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override bool CanDamage()
			=> projectile.ai[0] > 0;

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
		{
			this.DrawAroundOrigin(spriteBatch, lightColor * projectile.Opacity);
			return (false);
		}
	}
}
