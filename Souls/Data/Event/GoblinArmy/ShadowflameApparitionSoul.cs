using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.GoblinArmy
{
	public class ShadowflameApparitionSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.ShadowFlameApparition;
		public override string soulDescription => "Send out a barrage of apparitions.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 3 + stack;
			int damage = 45 + 5 * stack;

			int projType = ProjectileType<ShadowflameApparitionSoulProj>();
			Vector2 initialVelocity = Vector2.Normalize(Main.MouseWorld - p.Center);
			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(p.Center, initialVelocity.RotatedByRandom(MathHelper.PiOver2) * 4, projType, damage, .2f, p.whoAmI, initialVelocity.X * 8, initialVelocity.Y * 8);
			}

			return (true);
		}
	}

	public class ShadowflameApparitionSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.ShadowFlameApparition;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 6;
			DisplayName.SetDefault("Shadowflame Apparition");
		}
		public override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 24;

			projectile.penetrate = -1;
			projectile.timeLeft = 300;

			projectile.magic = true;
			projectile.friendly = true;

			drawOffsetX = -12;
			drawOriginOffsetY = -8;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				for (int i = 0; i < 5; ++i)
				{
					Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100)].noGravity = true;
				}
			}
			else if (projectile.localAI[0]++ >= 15)
			{
				projectile.velocity = Vector2.Lerp(projectile.velocity, new Vector2(projectile.ai[0], projectile.ai[1]), .05f);
			}

			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			projectile.spriteDirection = -projectile.direction;
			projectile.rotation = projectile.velocity.ToRotation() + (projectile.direction == -1 ? MathHelper.Pi : 0);
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.ShadowFlame, 120);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)].noGravity = true;
			}
		}
	}
}
