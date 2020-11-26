using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Data.Event.Sandstorm
{
	public class AngryTumblerSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Tumbleweed;
		public override string soulDescription => "Summons a tumbleweed that grows with distance.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 2;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<AngryTumblerSoulProj>(), 5 + stack, .1f, p.whoAmI, 0, (stack >= 9 ? 1 : 0));
			return (true);
		}
	}

	public class AngryTumblerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Tumbleweed;

		int defDamage;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tumbleweed");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.friendly = true;
			projectile.hostile = false;

			projectile.penetrate = 4;
			projectile.timeLeft = 240;
		}

		public override bool PreAI()
		{
			if (defDamage == 0)
				defDamage = projectile.damage;

			// Rolling behavior.
			if (projectile.velocity.Y == 0)
			{
				if (projectile.scale < 3f)
				{
					projectile.damage = (int)(defDamage * (projectile.scale * projectile.scale));
					projectile.scale += .01f;
					projectile.velocity.X *= 1.006f;
				}
			}

			// Rotate the projectile towards its X velocity.
			projectile.rotation += .2f * projectile.direction;

			// Gravity.
			projectile.velocity.Y += .05f;
			if (projectile.velocity.Y < 0)
				projectile.velocity.Y *= .99f;
			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			float stepSpeed = 20;

			// The projectile has collided, make it break.
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity = oldVelocity;
				projectile.velocity.Y = 0;
				Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width,
					projectile.height, ref stepSpeed, ref projectile.gfxOffY);
				if (projectile.velocity.X == 0)
					return (true);
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item10, projectile.position);
			for (int i = 0; i < (int)(20 * projectile.scale); i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Sandstorm, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];

			Vector2 origin = new Vector2(tex.Width * .5f, tex.Height * .5f);

			Vector2 position = projectile.position - new Vector2(0, 8 * (projectile.scale - 1));

			spriteBatch.Draw(tex, position + origin - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}
}
