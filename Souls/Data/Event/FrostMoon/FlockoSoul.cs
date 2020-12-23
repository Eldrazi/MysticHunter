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
	internal sealed class FlockoSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Flocko;
		public override string soulDescription => "Spawn falling Flockos around yourself.";

		public override short cooldown => 30;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60 + 5 * stack;

			int tries = 0;
			Vector2 spawnPos = Vector2.Zero;

			for (; tries < 10; tries++)
			{
				spawnPos = p.Center + new Vector2(Main.rand.Next(-200, 201), -300);
				if (!Collision.SolidCollision(spawnPos, 16, 16))
					break;
			}
			if (tries >= 10)
			{
				return (false);
			}

			int randomSide = Main.rand.Next(2) == 0 ? -1 : 1;
			Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<FlockoSoulProj>(), damage, .5f, p.whoAmI, 0, randomSide);
			return (true);
		}
	}

	internal sealed class FlockoSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Flocko;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flocko");
			Main.projFrames[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.scale = .6f;
			projectile.penetrate = 3;

			projectile.magic = true;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0] == 0)
			{
				projectile.localAI[0] += .05f;
				projectile.velocity.X = (float)Math.Sin(projectile.localAI[0]) * 3 * projectile.ai[1];

				projectile.velocity.Y += .1f;
				if (projectile.velocity.Y > 2)
					projectile.velocity.Y = 2;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					Vector2 direction = Main.npc[i].Center - projectile.Center;
					if (Main.npc[i].CanBeChasedBy(projectile) && direction.Length() <= 160)
					{
						projectile.ai[0] = i + 1;
						break;
					}
				}
			}
			else
			{
				NPC target = Main.npc[(int)projectile.ai[0] - 1];

				if (!target.CanBeChasedBy(projectile))
				{
					projectile.ai[0] = 0;
					return (false);
				}

				Vector2 desiredVelocity = Vector2.Normalize(target.Center - projectile.Center) * 12;

				projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, .08f);
			}

			projectile.rotation += projectile.velocity.X * .04f;
			if (Main.rand.Next(5) == 0)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 50);
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, 0, 0, 50);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
			Vector2 origin = frame.Size() / 2;

			for (int i = 0; i < projectile.oldPos.Length / 2; ++i)
			{
				float alpha = 1 - .2f * i;
				spriteBatch.Draw(texture, projectile.oldPos[i] + (origin / 2) * projectile.scale - Main.screenPosition,
					frame, lightColor * alpha, projectile.oldRot[i], origin, projectile.scale, SpriteEffects.None, 0);
			}

			spriteBatch.Draw(texture, projectile.position + (origin/2) * projectile.scale - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}
}
