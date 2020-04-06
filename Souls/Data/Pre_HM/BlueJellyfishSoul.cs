using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class BlueJellyfishSoul : BaseSoul
	{
		public override short soulNPC => NPCID.BlueJellyfish;
		public override string soulDescription => "Shoot a light emitting blob.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(5 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6;

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<BlueJellyfishSoulProj>(), 0, 0, p.whoAmI, 0, stack);
			return (true);
		}
	}

	public class BlueJellyfishSoulProj : ModProjectile
	{
		public override string Texture => "MysticHunter/Souls/Data/Pre_HM/BlueSlimeSoulProj";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blob");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;

			projectile.alpha = 75;
			projectile.timeLeft = 3600;

			projectile.friendly = true;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0] == 0)
			{
				if (projectile.velocity.Y == 0 && projectile.velocity.X != 0)
				{
					projectile.velocity.X = projectile.velocity.X * .97f;
					if (projectile.velocity.X > -.01f && projectile.velocity.X < .01f)
					{
						projectile.velocity.X = 0f;
						projectile.netUpdate = true;
					}
				}

				projectile.velocity.Y = projectile.velocity.Y + 0.2f;
				projectile.rotation += projectile.velocity.X * 0.1f;
			}

			Vector3 light = new Vector3(.3f * projectile.ai[1], .5f * projectile.ai[1], .8f * projectile.ai[1]);
			Lighting.AddLight(projectile.Center, light);

			return (false);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return (new Color(255, 255, 255, 0));
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.ai[0] == 0)
			{
				projectile.position += oldVelocity;
				if (oldVelocity.X != projectile.velocity.X)
				{
					if (oldVelocity.X >= 0)
						projectile.rotation = -MathHelper.PiOver2;
					else
						projectile.rotation = MathHelper.PiOver2;
				}
				if (oldVelocity.Y != projectile.velocity.Y)
				{
					if (oldVelocity.Y >= 0)
						projectile.rotation = 0;
					else
						projectile.rotation = (float)System.Math.PI;
				}
				projectile.frame = 1;
				projectile.ai[0] = 1;
				projectile.velocity *= 0;
			}

			return (false);
		}
		
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 180, Color.AliceBlue);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = GetTexture(Texture);

			Vector2 projOrigin = new Vector2(texture.Width * .5f, (texture.Height / Main.projFrames[projectile.type]) * .5f);

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(texture, 1, Main.projFrames[projectile.type], 0, projectile.frame)),
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}
	}
}
