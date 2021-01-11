#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.Rain
{
	public class AngryNimbusSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.AngryNimbus;
		public override string soulDescription => "Fire a bolt of chain lightning.";

		public override short cooldown => 480;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60 + 10 * stack;

			Vector2 projVel = Vector2.Normalize(Main.MouseWorld - p.Center) * 12;

			Projectile.NewProjectile(p.Center, projVel, ModContent.ProjectileType<AngryNimbusSoulProj>(), damage, 0, p.whoAmI, projVel.ToRotation());

			return (true);
		}
	}

	internal sealed class AngryNimbusSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chain Lightning");
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 8;
			projectile.scale = .5f;
			
			projectile.penetrate = -1;
			projectile.extraUpdates = 4;

			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;

			projectile.timeLeft = 120 * (projectile.extraUpdates + 1);
		}

		public override bool PreAI()
		{
			projectile.ai[1]--;
			projectile.frameCounter++;
			Lighting.AddLight(projectile.Center, .3f, .45f, .5f);

			if (projectile.velocity == Vector2.Zero)
			{
				// Check to see if the projectile has been able to move at all.
				// If not, kill the projectile.
				if (projectile.frameCounter >= projectile.extraUpdates * 2)
				{
					projectile.frameCounter = 0;
					bool hasProjectileMoved = false;
					for (int i = 1; i < projectile.oldPos.Length; i++)
					{
						if (projectile.oldPos[i] != projectile.oldPos[0])
						{
							hasProjectileMoved = true;
						}
					}

					if (hasProjectileMoved == false)
					{
						projectile.Kill();
						return (false);
					}
				}

				// Dust visual effects.
				if (Main.rand.Next(projectile.extraUpdates) == 0)
				{
					for (int i = 0; i < 2; ++i)
					{
						float newDustRot = projectile.rotation + ((Main.rand.Next(2) == 1) ? -1 : 1) * MathHelper.PiOver2;
						float newDustVelocityMod = Main.rand.NextFloat() * .8f + 1f;
						Vector2 newDustVel = new Vector2((float)Math.Cos(newDustRot) * newDustVelocityMod, (float)Math.Sin(newDustRot) * newDustVelocityMod);

						int newDust = Dust.NewDust(projectile.Center, 0, 0, 226, newDustVel.X, newDustVel.Y);
						Main.dust[newDust].noGravity = true;
						Main.dust[newDust].scale = 1.2f;
					}

					if (Main.rand.Next(5) == 0)
					{
						Vector2 newDustPos = projectile.velocity.RotatedBy(MathHelper.PiOver2) * (Main.rand.NextFloat() - .5f) * projectile.width;
						int newDust = Dust.NewDust(projectile.Center + newDustPos - Vector2.One * 4f, 8, 8, 31, 0, 0, 100, default, 1.5f);
						Main.dust[newDust].velocity *= .5f;
						Main.dust[newDust].velocity.Y = -Math.Abs(Main.dust[newDust].velocity.Y);
					}
				}
			}
			else if (projectile.frameCounter >= projectile.extraUpdates * 2)
			{
				projectile.frameCounter = 0;

				float velocityLength = projectile.velocity.Length();

				int iterator = 0;
				Vector2 up = -Vector2.UnitY;

				while (true)
				{
					int randNum = Main.rand.Next();
					randNum %= 100;
					float randomRot = randNum / 100f * MathHelper.TwoPi;

					Vector2 rotationalVector = randomRot.ToRotationVector2();
					if (rotationalVector.Y > 0)
					{
						rotationalVector.Y *= -1;
					}

					bool f = false;
					if (rotationalVector.Y > -.02f)
					{
						f = true;
					}
					if (rotationalVector.X * (projectile.extraUpdates + 1) * 2f * velocityLength + projectile.localAI[0] > 40f)
					{
						f = true;
					}
					if (rotationalVector.X * (projectile.extraUpdates + 1) * 2 * velocityLength + projectile.localAI[0] < -40f)
					{
						f = true;
					}

					if (!f)
					{
						up = rotationalVector;
						break;
					}

					int i = iterator++;
					if (i >= 100)
					{
						projectile.velocity = Vector2.Zero;
						projectile.localAI[1] = 1f;
						break;
					}
				}

				if (projectile.velocity != Vector2.Zero)
				{
					projectile.localAI[0] += up.X * (projectile.extraUpdates + 1) * 2f * velocityLength;
					projectile.velocity = up.RotatedBy(projectile.ai[0] + MathHelper.PiOver2) * velocityLength;
					projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
				}
			}
			return (false);
		}

		public override void PostAI()
		{
			if (projectile.frameCounter == 0 || projectile.oldPos[0] == Vector2.Zero)
			{
				for (int i = projectile.oldPos.Length - 1; i > 0; --i)
				{
					projectile.oldPos[i] = projectile.oldPos[i - 1];
				}
				projectile.oldPos[0] = projectile.position;
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (projectile.ai[1] > 0)
				return (false);

			for (int i = 0; i < projectile.oldPos.Length && (projectile.oldPos[i].X != 0f || projectile.oldPos[i].Y != 0f); ++i)
			{
				projHitbox.X = (int)projectile.oldPos[i].X;
				projHitbox.Y = (int)projectile.oldPos[i].Y;
				if (projHitbox.Intersects(targetHitbox))
				{
					return (true);
				}
			}

			return (base.Colliding(projHitbox, targetHitbox));
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.timeLeft < 120 || Main.myPlayer != projectile.owner)
				return;

			int chains = 2;
			for (int i = 0; i < Main.maxNPCs && chains > 0; ++i)
			{
				if (!Main.npc[i].active || i == target.whoAmI || Main.npc[i].friendly || projectile.Distance(Main.npc[i].Center) > projectile.timeLeft * projectile.velocity.Length())
					continue;

				Vector2 projVel = Vector2.Normalize(Main.npc[i].Center - projectile.Center) * 12;

				chains--;
				int newProj = Projectile.NewProjectile(projectile.Center, projVel, ModContent.ProjectileType<AngryNimbusSoulProj>(), damage, 0, projectile.owner, projVel.ToRotation(), 30);
				Main.projectile[newProj].timeLeft = projectile.timeLeft;
				NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, newProj);
			}
			projectile.velocity *= 0;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			return (projectile.localAI[1] != 1);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.localAI[1] < 1f)
			{
				projectile.localAI[1] += 2f;
				projectile.position += projectile.velocity;
				projectile.velocity = Vector2.Zero;
			}
			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 end = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
			Texture2D texture = TextureAssets.Extra[33].Value;
			projectile.GetAlpha(lightColor);
			Vector2 scale = new Vector2(projectile.scale) * .5f;

			for (int i = 0; i < 3; ++i)
			{
				if (i == 0)
				{
					scale = new Vector2(projectile.scale) * .6f;
					DelegateMethods.c_1 = new Color(115, 204, 219, 0) * .5f;
				}
				else if (i == 1)
				{
					scale = new Vector2(projectile.scale) * .4f;
					DelegateMethods.c_1 = new Color(113, 251, 255, 0) * .5f;
				}
				else
				{
					scale = new Vector2(projectile.scale) * .2f;
					DelegateMethods.c_1 = new Color(255, 255, 255, 0) * .5f;
				}
				DelegateMethods.f_1 = 1f;

				for (int j = projectile.oldPos.Length - 1; j > 0; --j)
				{
					if(projectile.oldPos[j] != Vector2.Zero)
					{
						Vector2 start = projectile.oldPos[j] + projectile.Size / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
						Vector2 end2 = projectile.oldPos[j - 1] + projectile.Size / 2 + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
						Utils.DrawLaser(spriteBatch, texture, start, end2, scale, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
					}
				}

				if (projectile.oldPos[0] != Vector2.Zero)
				{
					DelegateMethods.f_1 = 1f;
					Vector2 start = projectile.oldPos[0] + projectile.Size / 2 + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
					Utils.DrawLaser(spriteBatch, texture, start, end, scale, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
				}
			}

			return (false);
		}
	}
}
