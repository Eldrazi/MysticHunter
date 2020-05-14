using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class FungoFishSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.FungoFish;
		public override string soulDescription => "Fire a cone of spores.";

		public override short cooldown => 20;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			Vector2 desiredVeloity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6;

			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(p.Center, desiredVeloity.RotatedByRandom(MathHelper.PiOver4), ProjectileType<FungoFishSoulProj>(), 20 + 5 * stack, 0, p.whoAmI);
			}
			return (true);
		}
	}

	public class FungoFishSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.TruffleSpore;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spore");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;

			projectile.alpha = 255;
			projectile.penetrate = 1;
			projectile.timeLeft = 360;

			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (++projectile.frameCounter >= 4)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			if (projectile.alpha > 0 || projectile.timeLeft <= 30)
			{
				if (projectile.timeLeft <= 30)
					projectile.alpha += 8;
				else
					projectile.alpha -= 15;
			}
			else
			{
				projectile.alpha = 0;

				float randomGlowModifier = Main.rand.Next(28, 42) * 0.005f;
				randomGlowModifier += (270 - Main.mouseTextColor) / 500f;
				float r = 0.1f;
				float g = 0.3f + randomGlowModifier / 2f;
				float b = 0.6f + randomGlowModifier;
				Lighting.AddLight(projectile.Center, new Vector3(r, g, b) * .35f);
			}

			if (projectile.velocity.Length() > .5)
				projectile.velocity *= .95f;
			else
				projectile.velocity = new Vector2(0, (float)Math.Sin(MathHelper.TwoPi * projectile.ai[0] / 180) * .2f);
			projectile.ai[0] = (projectile.ai[0] + 1) % 180;
			projectile.rotation = projectile.velocity.X * .04f;
			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle frame = texture.Frame(3, 1, projectile.frame, 0);
			Vector2 origin = frame.Size() / 2f;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, lightColor * projectile.Opacity, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(Main.glowMaskTexture[168], projectile.Center - Main.screenPosition, frame, new Color(127 - projectile.alpha / 2, 127 - projectile.alpha / 2, 127 - projectile.alpha / 2, 0), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
			return (false);
		}
	}
}
