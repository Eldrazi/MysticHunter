#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class GigazapperSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GigaZapper;
		public override string soulDescription => "Zap enemies with an electric spear.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 90 + 10 * stack;

			Vector2 projVel = new Vector2(p.direction * 6, 0);
			Projectile.NewProjectile(p.Center, projVel, ModContent.ProjectileType<GigazapperSoulProj>(), damage, 1f, p.whoAmI);
			return (true);
		}
	}

	internal sealed class GigazapperSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GigaZapperSpear;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Zap Spear");
			Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.GigaZapperSpear];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 10;

			projectile.scale = .8f;
			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1] == 0)
			{
				projectile.ai[1] = 1;
				SoundEngine.PlaySound(SoundID.Item12, projectile.position);
			}

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				for (int i = 0; i < 4; ++i)
				{
					Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, projectile.velocity.X)];
					newDust.position = Vector2.Lerp(newDust.position, projectile.Center, .25f);
					newDust.scale = .5f;
					newDust.noGravity = true;
					newDust.velocity *= .5f;
					newDust.velocity += projectile.velocity * .66f;
				}
			}

			if (projectile.ai[0] < 16)
			{
				for (int i = 0; i < 2; ++i)
				{
					Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, projectile.velocity.X)];
					newDust.position = projectile.position + new Vector2((projectile.direction == 1 ? 1 : 0) * projectile.width, 2 + (projectile.height - 4) * i);
					newDust.scale = .3f;
					newDust.noGravity = true;
					newDust.velocity = Vector2.Zero;
				}
			}

			if (projectile.ai[0]++ >= 12)
			{
				if (projectile.ai[0] >= 20)
				{
					projectile.Kill();
				}
				projectile.alpha += 30;
			}

			return (false);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - (float)projectile.alpha / 255f);
		}
	}
}
