#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;
using System.IO;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class TwinklePopperSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.StardustSpiderBig;
		public override string soulDescription => "Summon a stationary Twinkle Popper";

		public override short cooldown => 200;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Collision.SolidCollision(Main.MouseWorld, 32, 32))
			{
				return (false);
			}

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<TwinklePopperSoul_Proj>())
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			int damage = 210 + 20 * stack;
			Projectile.NewProjectile(Main.MouseWorld, default, ModContent.ProjectileType<TwinklePopperSoul_Proj>(), damage, 0, p.whoAmI);

			return (true);
		}
	}

	public class TwinklePopperSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.StardustSpiderBig;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Twinkle Popper");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.StardustSpiderBig];
		}
		public override void SetDefaults()
		{
			projectile.width = 36;
			projectile.height = 34;

			projectile.timeLeft = Projectile.SentryLifeTime;

			projectile.sentry = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.StardustSpiderBig)
			{
				projectile.timeLeft = 2;
			}

			// Spawn.
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlaySound(SoundID.Item46, projectile.position);

				for (int i = 0; i < 80; ++i)
				{
					Dust newDust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 135);
					newDust.velocity *= 2f;
					newDust.noGravity = true;
					newDust.scale *= 1.15f;
				}
			}

			projectile.velocity.X = 0f;
			projectile.velocity.Y += 0.2f;
			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}

			if (projectile.ai[0] == 0)
			{
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC npc = Main.npc[i];

					if (!npc.active || !npc.CanBeChasedBy(projectile) || !Collision.CanHitLine(projectile.Center, 1, 1, npc.Center, 1, 1) ||
						projectile.Distance(npc.Center) > 600)
					{
						continue;
					}

					projectile.ai[0] = 1;
					break;
				}

				projectile.frame = 9;
			}
			else
			{
				projectile.ai[1]++;

				if (projectile.ai[1] >= 90)
				{
					projectile.frame = 10;
				}

				if (projectile.ai[1] >= 120)
				{
					projectile.ai[0] = 0;
					projectile.ai[1] = 0;

					if (projectile.owner == Main.myPlayer)
					{
						Projectile.NewProjectile(projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 4, ModContent.ProjectileType<TwinklePopperSoul_ProjSummon>(), projectile.damage, 0.2f, projectile.owner);
					}
				}
			}

			return (false);
		}

		public override bool CanDamage()
			=> false;

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
			=> false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 135, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f);
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}

	public class TwinklePopperSoul_ProjSummon : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.StardustSpiderSmall;

		int target, oldTarget;
		private readonly float maxTargetingRange = 600;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Twinkle");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.StardustSpiderSmall];
		}
		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 16;

			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.ignoreWater = true;

			target = oldTarget = 200;
		}

		public override bool PreAI()
		{
			int num = 30;
			int num2 = 10;
			bool flag2 = false;
			bool flag3 = false;

			if (projectile.velocity.Y == 0f && System.Math.Sign(projectile.velocity.X) != projectile.direction)
			{
				flag2 = true;
				projectile.localAI[0] += 1f;
			}

			if (projectile.position.X == projectile.oldPosition.X || projectile.localAI[0] >= num || flag2)
			{
				projectile.localAI[0] += 1f;
				flag3 = true;
			}
			else if (projectile.localAI[0] > 0f)
			{
				projectile.localAI[0] -= 1f;
			}

			if (projectile.localAI[0] > (num * num2))
			{
				projectile.localAI[0] = 0f;
			}
			if (projectile.localAI[0] == num)
			{
				projectile.netUpdate = true;
			}

			Vector2 vector = projectile.Center;
			
			float num3 = Main.npc[target].position.X + Main.npc[target].width * 0.5f - vector.X;
			float num4 = Main.npc[target].position.Y - vector.Y;
			float num5 = (float)System.Math.Sqrt(num3 * num3 + num4 * num4);
			if (num5 < 200f && !flag3)
			{
				projectile.localAI[0] = 0f;
			}

			projectile.ai[1] += 1f;
			bool canExplode = projectile.ai[1] >= 240f;
			if (!canExplode && projectile.velocity.Y == 0f)
			{
				for (int j = 0; j < Main.maxNPCs; j++)
				{
					if (Main.npc[j].active && Main.npc[j].Distance(projectile.Center) < 800f &&
						Main.npc[j].Center.Y < projectile.Center.Y && System.Math.Abs(Main.npc[j].Center.X - projectile.Center.X) < 20f)
					{
						canExplode = true;
						break;
					}
				}
			}

			if (canExplode && projectile.owner == Main.myPlayer)
			{
				for (int k = 0; k < 3; k++)
				{
					Vector2 newProjVelocity = new Vector2((Main.rand.NextFloat() - 0.5f) * 2f, -4f - 10f * Main.rand.NextFloat());
					Projectile newProj = Projectile.NewProjectileDirect(projectile.Center, newProjVelocity, ProjectileID.Twinkle, projectile.damage, 0f, projectile.owner);
					newProj.friendly = true;
					newProj.hostile = false;
				}
				projectile.Kill();
				return (false);
			}

			if (projectile.localAI[0] < num)
			{
				TargetClosest();
			}
			else
			{
				if (projectile.velocity.X == 0f)
				{
					if (projectile.velocity.Y == 0f)
					{
						projectile.ai[0] += 1f;
						if (projectile.ai[0] >= 2f)
						{
							projectile.direction *= -1;
							projectile.spriteDirection = projectile.direction;
							projectile.ai[0] = 0f;
						}
					}
				}
				else
				{
					projectile.ai[0] = 0f;
				}

				if (projectile.direction == 0)
				{
					projectile.direction = 1;
				}
			}

			if (projectile.velocity.Y == 0f || projectile.wet || System.Math.Abs(projectile.velocity.X) == projectile.direction)
			{
				if (System.Math.Sign(projectile.velocity.X) != projectile.direction)
				{
					projectile.velocity.X *= 0.9f;
				}
				float speed = 6f;
				float acceleration = 0.2f;

				if (projectile.velocity.X < -speed || projectile.velocity.X > speed)
				{
					if (projectile.velocity.Y == 0f)
					{
						projectile.velocity *= 0.8f;
					}
				}
				else if (projectile.velocity.X < speed && projectile.direction == 1)
				{
					projectile.velocity.X += acceleration;
					if (projectile.velocity.X > speed)
					{
						projectile.velocity.X = speed;
					}
				}
				else if (projectile.velocity.X > 0f - speed && projectile.direction == -1)
				{
					projectile.velocity.X -= acceleration;
					if (projectile.velocity.X < 0f - speed)
					{
						projectile.velocity.X = 0f - speed;
					}
				}
			}

			if (projectile.velocity.Y >= 0f)
			{
				Vector2 position = projectile.position;
				position.X += projectile.velocity.X;

				int x = (int)((position.X + (projectile.width / 2) + ((projectile.width / 2 + 1) * System.Math.Sign(projectile.velocity.X))) / 16f);
				int y = (int)((position.Y + projectile.height - 1f) / 16f);

				Tile t1 = Framing.GetTileSafely(x, y);
				Tile t2 = Framing.GetTileSafely(x, y - 1);
				Tile t3 = Framing.GetTileSafely(x, y - 4);
				Tile t4 = Framing.GetTileSafely(x, y - 2);
				Tile t5 = Framing.GetTileSafely(x, y - 3);
				Tile t6 = Framing.GetTileSafely(x - System.Math.Sign(projectile.velocity.X), y - 3);

				if ((x * 16) < position.X + projectile.width && (x * 16f + 16f) > position.X &&
					((t1.nactive() && !t1.topSlope() && !t2.topSlope() && Main.tileSolid[t1.type] && !Main.tileSolidTop[t1.type]) ||
					(t2.halfBrick() && t2.nactive())) && (!t2.nactive() || !Main.tileSolid[t2.type] || Main.tileSolidTop[t2.type] ||
					(t2.halfBrick() && (!t3.nactive() || !Main.tileSolid[t3.type] || Main.tileSolidTop[t3.type]))) &&
					(!t4.nactive() || !Main.tileSolid[t4.type] || Main.tileSolidTop[t4.type]) &&
					(!t5.nactive() || !Main.tileSolid[t5.type] || Main.tileSolidTop[t5.type]) &&
					(!t6.nactive() || !Main.tileSolid[t6.type]))
				{
					float num13 = y * 16;
					if (t1.halfBrick())
					{
						num13 += 8f;
					}
					if (t2.halfBrick())
					{
						num13 -= 8f;
					}

					if (num13 < position.Y + projectile.height)
					{
						float num14 = position.Y + projectile.height - num13;
						if (num14 <= 16.1f)
						{
							projectile.gfxOffY += projectile.position.Y + projectile.height - num13;
							projectile.position.Y = num13 - projectile.height;
							if (num14 < 9f)
							{
								projectile.stepSpeed = 1f;
							}
							else
							{
								projectile.stepSpeed = 2f;
							}
						}
					}
				}
			}

			if (projectile.velocity.Y == 0f)
			{
				int num15 = (int)((projectile.position.X + (projectile.width / 2) + ((projectile.width / 2 + 2) * projectile.direction) + projectile.velocity.X * 5f) / 16f);
				int num16 = (int)((projectile.position.Y + projectile.height - 15f) / 16f);
				int num17 = -projectile.spriteDirection;
				if (System.Math.Sign(projectile.velocity.X) == num17)
				{
					Tile t1 = Framing.GetTileSafely(num15, num16);
					Tile t2 = Framing.GetTileSafely(num15, num16 - 1);
					Tile t3 = Framing.GetTileSafely(num15, num16 - 2);
					Tile t4 = Framing.GetTileSafely(num15, num16 - 3);

					if (t3.nactive() && Main.tileSolid[t3.type])
					{
						if (t4.nactive() && Main.tileSolid[t4.type])
						{
							projectile.velocity.Y = -8.5f;
							projectile.netUpdate = true;
						}
						else
						{
							projectile.velocity.Y = -7.5f;
							projectile.netUpdate = true;
						}
					}
					else if (t2.nactive() && !t2.topSlope() && Main.tileSolid[t2.type])
					{
						projectile.velocity.Y = -7f;
						projectile.netUpdate = true;
					}
					else if (projectile.position.Y + projectile.height - (num16 * 16) > 20f && t1.nactive() && !t1.topSlope() && Main.tileSolid[t1.type])
					{
						projectile.velocity.Y = -6f;
						projectile.netUpdate = true;
					}
				}

				// Animation.
				projectile.frameCounter++;
				if (projectile.frameCounter >= 6.0)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				}
			}

			// Gravity.
			projectile.velocity.Y += 0.2f;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 135, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f);
				d.noGravity = true;
			}
		}

		private void TargetClosest()
		{
			this.target = 200;

			float currentTargetingRange = maxTargetingRange;
			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				NPC npc = Main.npc[i];

				if (!npc.active || !npc.CanBeChasedBy(projectile))
				{
					continue;
				}

				float distance = (npc.Center - projectile.Center).Length();
				if (distance < currentTargetingRange)
				{
					this.target = i;
				}
			}

			projectile.direction = System.Math.Sign(Main.npc[this.target].Center.X - projectile.Center.X);

			if (target != oldTarget)
			{
				projectile.netUpdate = true;
				this.oldTarget = this.target;
			}
		}

		public override bool CanDamage()
			=> false;

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
			=> false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(this.target);
			writer.Write(projectile.localAI[0]);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			this.target = reader.ReadInt32();
			projectile.localAI[0] = reader.ReadSingle();
		}
	}
}
