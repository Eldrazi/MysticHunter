#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	internal sealed class KrampusSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Krampus;
		public override string soulDescription => "Toss an exploding present.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(4 + 1 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 80;
			int modifier = 128;

			if (stack >= 5)
			{
				damage += 15;
				modifier += 64;
			}
			if (stack >= 9)
			{
				damage += 15;
				modifier += 64;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 8;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<KrampusSoulProj>(), damage, 1, p.whoAmI, 0, modifier);
			return (true);
		}
	}

	internal sealed class KrampusSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Item_" + ItemID.Present;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Present");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = -1;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0]++ >= 18)
			{
				projectile.velocity.X *= .99f;
				projectile.velocity.Y += .28f;
			}

			projectile.rotation += projectile.velocity.Length() * .05f * projectile.direction;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, projectile.position);

			for (int i = 0; i < 20; ++i)
			{
				int randDustType = Utils.SelectRandom<int>(Main.rand, 139, 140, 141, 142);
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, randDustType, 0, 0, 100, default, 1.5f)];
				newDust.velocity *= 1.4f;
			}

			for (int i = 0; i < 10; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.FlameBurst, 0, 0, 100, default, 2.5f)];
				newDust.noGravity = true;
				newDust.velocity *= 5;

				newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.FlameBurst, 0, 0, 100, default, 1.5f)];
				newDust.velocity *= 3;
			}

			projectile.position += projectile.Size / 2;
			projectile.width = projectile.height = (int)projectile.ai[1];
			projectile.position -= projectile.Size / 2;

			projectile.Damage();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 origin = texture.Size() / 2;

			spriteBatch.Draw(texture, projectile.position + origin/2 - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}
}
