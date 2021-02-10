#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class SrollerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SolarSroller;
		public override string soulDescription => "Toss a Sroller.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 230 + 20 * stack;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<SrollerSoul_Proj>(), damage, 0.5f + 0.1f * stack, p.whoAmI);

			return (true);
		}
	}

	public class SrollerSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.SolarSroller;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sroller");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.SolarSroller];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 34;

			projectile.timeLeft = 240;
			projectile.penetrate = -1;

			projectile.friendly = true;

			this.drawOriginOffsetY = 6;
		}

		public override bool PreAI()
		{
			if (System.Math.Abs(projectile.velocity.X) < 8 && projectile.velocity.Y == 0)
			{
				projectile.velocity.X += 0.04f * projectile.direction;
			}

			if (++projectile.ai[0] >= 15)
			{
				projectile.velocity.Y += 0.2f;
			}
			projectile.frame = 13;
			projectile.spriteDirection = -projectile.direction;

			projectile.rotation += projectile.velocity.X * 0.05f;

			if (Main.rand.Next(10) == 0)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare);
				newDust.noGravity = true;
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X * 0.8f;
				projectile.velocity.Y = -4f;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0, 0, 100)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
