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
	public class DrManFlySoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.DrManFly;
		public override string soulDescription => "Throw a vial that inflicts a random debuff.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 70 + 10 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<DrManFlySoul_Proj>(), damage, 2f, p.whoAmI, 0);

			return (true);
		}
	}

	public class DrManFlySoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.ToxicFlask;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flask");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		private int[] debuffs = new int[] { BuffID.Venom, BuffID.Ichor, BuffID.Poisoned, BuffID.Frostburn, BuffID.ShadowFlame, BuffID.Confused };
		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					projectile.netUpdate = true;
					projectile.ai[1] = Main.rand.Next(debuffs.Length);
				}
				projectile.localAI[0] = 1;
			}

			if (projectile.timeLeft <= 3)
			{
				projectile.alpha = 255;

				projectile.position = projectile.Center;
				projectile.width = projectile.height = 128;
				projectile.Center = projectile.position;
			}
			else
			{
				if (++projectile.ai[0] >= 15)
				{
					projectile.velocity.X *= 0.98f;
					projectile.velocity.Y += 0.2f;
				}

				projectile.rotation += projectile.velocity.X * 0.05f;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Explode();
			target.AddBuff(debuffs[(int)projectile.ai[1]], 600);
		}
		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			Explode();
			target.AddBuff(debuffs[(int)projectile.ai[1]], 600);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Explode();
			return (false);
		}

		private void Explode()
			=> projectile.timeLeft = (projectile.timeLeft > 3 ? 3 : projectile.timeLeft);

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 40; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.ToxicBubble, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = 2;
				}
				d.velocity *= 1.3f;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor * projectile.Opacity);
	}
}
