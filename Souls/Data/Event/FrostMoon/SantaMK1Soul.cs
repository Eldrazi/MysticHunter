#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	internal sealed class SantaMK1Soul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SantaNK1;
		public override string soulDescription => "Shoot a missile barrage";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60;
			float modifier = 1;
			int amount = 5;

			if (stack >= 5)
			{
				damage += 30;
				modifier++;
				amount -= 2;
			}
			if (stack >= 9)
			{
				damage += 50;
				modifier++;
				amount -= 2;
			}

			int direction = Math.Sign(Main.MouseWorld.X - p.Center.X);
			for (int i = 0; i < amount; ++i)
			{
				Vector2 velocity = new Vector2((6 - 1 * i) * direction, -5);

				Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<SantaMK1Soul_Proj>(), damage, 7f, p.whoAmI, modifier, direction);
			}

			return (true);
		}
	}

	internal sealed class SantaMK1Soul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.RocketI;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rocket");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 20;

			projectile.ranged = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (++projectile.localAI[0] >= 25)
			{
				projectile.velocity.Y += 0.2f;
				projectile.tileCollide = true;
			}

			projectile.rotation = projectile.velocity.ToRotation();
			if (projectile.ai[0] == 1)
			{
				projectile.rotation += MathHelper.PiOver2;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			projectile.position = projectile.Center;
			projectile.width = projectile.height = 64 * (int)projectile.ai[0];
			projectile.Center = projectile.position;

			for (int i = 0; i < 30; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 1.4f;
			}

			for (int i = 0; i < 20; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f)];
				newDust.noGravity = true;
				newDust.velocity *= 7f;

				newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 3f;
			}

			for (int i = 0; i < 2; ++i)
			{
				float num729 = i == 1 ? 0.8f : 0.4f;

				Gore newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				newGore.velocity *= num729;
				newGore.velocity += Vector2.One;

				newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				newGore.velocity *= num729;
				newGore.velocity.X -= 1f;
				newGore.velocity.Y += 1f;

				newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				newGore.velocity *= num729;
				newGore.velocity.X += 1f;
				newGore.velocity.Y -= 1f;

				newGore = Main.gore[Gore.NewGore(projectile.position, default, Main.rand.Next(61, 64))];
				newGore.velocity *= num729;
				newGore.velocity -= Vector2.One;
			}

			Main.PlaySound(SoundID.Item62, projectile.position);

			projectile.Damage();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture;

			switch ((int)projectile.ai[0])
			{
				case 1:
					texture = ModContent.GetTexture(Texture);
					break;

				case 2:
					texture = ModContent.GetTexture("MysticHunter/Souls/Data/Event/FrostMoon/SantaMK1Soul_Proj2");
					break;

				default:
					texture = ModContent.GetTexture("MysticHunter/Souls/Data/Event/FrostMoon/SantaMK1Soul_Proj3");
					break;
			}

			Vector2 origin = texture.Size() / 2;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
