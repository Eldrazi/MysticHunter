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
	public class FritzSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Fritz;
		public override string soulDescription => "Toss a Fritz at your enemies.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 50 + 10 * stack;
			float knockBack = 1.25f + 0.05f * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8f;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<FritzSoul_Proj>(), damage, knockBack, p.whoAmI);

			return (true);
		}
	}

	public class FritzSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Fritz;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fritz");
			
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.Fritz];
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
			if (projectile.ai[0]++ >= 20)
			{
				projectile.velocity.X *= 0.98f;
				projectile.velocity.Y += 0.2f;
			}

			projectile.spriteDirection = -projectile.direction;
			projectile.rotation += projectile.velocity.X * 0.1f;

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = oldVelocity.X * -1;
			}
			if (projectile.velocity.Y != oldVelocity.Y || projectile.velocity.Y == 0f)
			{
				projectile.velocity.Y = oldVelocity.Y * -0.7f;
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; ++i)
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
			this.DrawAroundOrigin(spriteBatch, lightColor);
			return (false);
		}
	}
}
