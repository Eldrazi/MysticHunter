#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.Rain
{
	public class IceGolemSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.IceGolem;
		public override string soulDescription => "Fire an ice laser.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 70 + 10 * stack;
			int debuffChance = 10 + 5 * stack;

			Vector2 projVel = Vector2.Normalize(Main.MouseWorld - p.Center) * 12;

			Projectile.NewProjectile(p.Center, projVel, ModContent.ProjectileType<IceGolemSoulProj>(), damage, .1f, p.whoAmI, debuffChance);

			return (true);
		}
	}

	internal sealed class IceGolemSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EyeLaser;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ice Laser");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 4;
			
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 2;

			projectile.friendly = true;

			projectile.alpha = 255;
			projectile.scale = 1.7f;
		}

		public override bool PreAI()
		{
			if(projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				SoundEngine.PlaySound(SoundID.Item33, projectile.position);
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (projectile.alpha > 0)
			{
				projectile.alpha -= 20;
			}
			else
			{
				projectile.alpha = 0;
			}

			float light = .75f;
			Lighting.AddLight(projectile.Center, light * 0, light * 1, light * .7f);

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Slow, 300);

			if (Main.rand.Next(100) <= (int)projectile.ai[0])
			{
				target.AddBuff(BuffID.Frozen, 300);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			lightColor.R = (byte)(lightColor.R * .1f);
			lightColor.G = (byte)(lightColor.G * .8f);
			return (lightColor * projectile.Opacity);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, projectile.position);
			return (true);
		}
	}
}
