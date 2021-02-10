#region Using directives

using System;

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
	public class ReaperSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Reaper;
		public override string soulDescription => "Spawn spinning scythes around you.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 35 + 5 * stack;

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<ReaperSoul_Proj>(), damage, 0.5f, p.whoAmI);

			return (true);
		}
	}

	public class ReaperSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.DeathSickle;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scythe");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.timeLeft = 240;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.ownerHitCheck = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			projectile.ai[0] += MathHelper.Pi / 120;
			projectile.position = owner.position + new Vector2((float)Math.Cos(projectile.ai[0]), (float)Math.Sin(projectile.ai[0])) * 52;			

			projectile.rotation += 0.25f;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0f, 0f, 100)];
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
