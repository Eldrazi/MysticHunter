#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.PumpkinMoon
{
	public class SplinterlingSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Splinterling;
		public override string soulDescription => "Leave behind splinters while walking.";

		public override short cooldown => 20;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 20 + 2 * stack;

			if (p.velocity.X != 0 && p.velocity.Y >= -0.5f && p.velocity.Y <= 0.5f)
			{
				Vector2 newProjPos = p.position + new Vector2(8 - 16 * p.direction, p.height - 10);
				Vector2 velocity = Vector2.UnitX * -p.direction;
				Projectile.NewProjectile(newProjPos, velocity, ModContent.ProjectileType<SplinterlingSoulProj>(), damage, 0, p.whoAmI);
			}

			return (true);
		}
	}

	public class SplinterlingSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SpikyBall;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Splinter");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;

			projectile.aiStyle = 14;
			projectile.penetrate = 6;

			projectile.friendly = true;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (false);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X * .8f;
				if (Math.Abs(projectile.velocity.X) < .025f)
				{
					projectile.velocity.X = 0;
				}
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y * .8f;
				if (Math.Abs(projectile.velocity.Y) < .025f)
				{
					projectile.velocity.Y = 0;
				}
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_BorealWood, 0f, 0f, 100, default, 1f);
			}
		}
	}
}
