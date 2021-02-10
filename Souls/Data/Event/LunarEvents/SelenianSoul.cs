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
	public class SelenianSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SolarSolenian;
		public override string soulDescription => "Summon solar blades around you.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 35;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 200;
			int amount = 1;

			if (stack >= 5)
			{
				damage += 20;
				amount++;
			}
			if (stack >= 9)
			{
				damage += 30;
				amount += 2;
			}

			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(p.Center, default, ModContent.ProjectileType<SelenianSoul_Proj>(), damage, 1f, p.whoAmI, amount, i);
			}

			return (true);
		}
	}

	public class SelenianSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Glow_154";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Blade");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.SolarSolenian];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.penetrate = 3;
			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			projectile.scale = (projectile.ai[0] + 1) * 0.2f;

			projectile.localAI[0]++;
			float positionalRotationModifier = MathHelper.TwoPi / projectile.ai[0] * projectile.ai[1];
			positionalRotationModifier += MathHelper.TwoPi / 60 * projectile.localAI[0];

			Vector2 desiredPosition = owner.Center;
			desiredPosition += new Vector2((float)System.Math.Cos(positionalRotationModifier), (float)System.Math.Sin(positionalRotationModifier)) * 50;

			projectile.Center = desiredPosition;

			// Animation.
			if (++projectile.frameCounter >= 3)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 9;
			}
			if (projectile.frame < 5)
			{
				projectile.frame = 5;
			}

			if (Main.rand.Next(10) == 0)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f);
				newDust.noGravity = true;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.SolarFlare, 0, 0, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
