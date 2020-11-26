using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using IL.Terraria.GameContent.Achievements;

namespace MysticHunter.Souls.Data.Bosses
{
	public sealed class SkeletronPrimeSoul : PostHMSoul, IBossSoul
	{
		internal int[] PrimeSoulTypes = new int[] { NPCID.PrimeVice, NPCID.PrimeSaw, NPCID.PrimeLaser, NPCID.PrimeCannon };

		public override short soulNPC => NPCID.SkeletronPrime;
		public override string soulDescription => "Summon ultra-protective arms.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 35;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<SkeletronSoulProj>())
					Main.projectile[i].Kill();

			int damage = 50;

			int amount = 1;
			if (stack >= 3)
				amount++;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			for (int i = 0; i < amount; ++i)
			{
				int yHand = (i - 2 < 0 ? 1 : -1);
				int xHand = (i % 2 == 0 ? -1 : 1);

				int proj = Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<SkeletronPrimeSoulProj>(), damage, .2f, p.whoAmI, xHand, yHand);
				((SkeletronPrimeSoulProj)Main.projectile[proj].modProjectile).primeType = PrimeSoulTypes[i];
				Main.projectile[proj].netUpdate = true;
			}
			return (true);
		}
	}

	internal class SkeletronPrimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_0";

		private bool attacking
		{
			get { return projectile.localAI[0] == 1.0f; }
			set 
			{
				projectile.localAI[0] = (value == true ? 1f : 0f);
				projectile.netUpdate = true; 
			}
		}

		private Vector2 targetHoverPosition
		{
			get
			{
				Vector2 position = Main.player[projectile.owner].position + new Vector2(Main.player[projectile.owner].width * .5f, 0);

				position.X += 230 * projectile.scale * projectile.ai[0];
				position.Y += 130 * projectile.scale * projectile.ai[1];

				return (position);
			}
		}

		private int target;

		public int primeType = 0;

		private readonly float damping = .001f;
		private readonly float yAcceleration = .12f, xAcceleration = .2f;
		private readonly float maxTargetingDistance = 320;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mechanical Hand");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 52;

			projectile.scale = .6f;
			projectile.penetrate = -1;
			projectile.minionSlots = 0f;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override void AI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoul?.soulNPC == NPCID.SkeletronPrime)
				projectile.timeLeft = 2;

			// Set direction based on which side of the player this projectile is on.
			projectile.spriteDirection = -(int)projectile.ai[0];

			if (!attacking)
			{
				float yLength = Math.Min(Math.Abs(projectile.position.Y - targetHoverPosition.Y) * damping, yAcceleration);
				float xLength = Math.Min(Math.Abs(projectile.position.X - targetHoverPosition.X) * damping, xAcceleration);

				// Movement.
				if (projectile.position.Y > targetHoverPosition.Y)
				{
					if (projectile.velocity.Y > 0)
						projectile.velocity.Y *= .96f;
					projectile.velocity.Y -= yLength;
				}
				else if (projectile.position.Y < targetHoverPosition.Y)
				{
					if (projectile.velocity.Y < 0)
						projectile.velocity.Y *= .96f;
					projectile.velocity.Y += yLength;
				}

				if (projectile.Center.X > targetHoverPosition.X)
				{
					if (projectile.velocity.X > 0)
						projectile.velocity.X *= .96f;
					projectile.velocity.X -= xLength;
				}
				else if (projectile.Center.X < targetHoverPosition.X)
				{
					if (projectile.velocity.X < 0)
						projectile.velocity.X *= .96f;
					projectile.velocity.X += xLength;
				}

				// Clamp velocity.
				projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -6, 6);
				projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -8, 8);

				// Setting correct rotation.
				Vector2 ownerPos = owner.position + new Vector2(owner.width * .5f, owner.height * .5f);
				Vector2 dir = ownerPos + new Vector2(-200 * projectile.ai[0], 60 * projectile.ai[1]) - projectile.Center;
				projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;

				// Finding target.
				if (projectile.localAI[1]++ >= 320)
				{
					this.target = 255;
					float currentDist = maxTargetingDistance;

					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						if (Main.npc[i].CanBeChasedBy(projectile))
						{
							float d = Vector2.Distance(Main.npc[i].Center, projectile.Center);
							if (d <= currentDist)
							{
								currentDist = d;
								this.target = i;
							}
						}
					}

					if (this.target != 255)
					{
						this.attacking = true;
						projectile.localAI[1] = 0;
						projectile.netUpdate = true;
					}
				}
			}
			else
			{
				NPC npc = Main.npc[target];

				if (!npc.active)
				{
					this.attacking = false;
					projectile.localAI[1] = 0;
					projectile.netUpdate = true;
					return;
				}

				// Close combat attack for both the vice and the saw.
				if (primeType == NPCID.PrimeVice || primeType == NPCID.PrimeSaw)
				{
					if (projectile.localAI[1] == 0)
					{
						// Set correct rotation.
						Vector2 targetPos = owner.position + new Vector2(owner.width * .5f - 200 * projectile.ai[0], 230) - projectile.Center;
						projectile.rotation = targetPos.ToRotation() + 1.57f;

						projectile.velocity.X *= .95f;
						projectile.velocity.Y -= .1f;

						if (projectile.velocity.Y < -8)
							projectile.velocity.Y = -8;

						if (projectile.position.Y < owner.position.Y - 200f)
						{
							projectile.localAI[1] = 1;
							projectile.netUpdate = true;
							projectile.velocity = Vector2.Normalize(npc.Center - projectile.Center) * 18;
						}
					}
					else if (projectile.localAI[1] == 1)
					{
						if (projectile.position.Y > npc.position.Y || projectile.velocity.Y < 0f)
							projectile.localAI[1] = 2;
					}
					else if (projectile.localAI[1] == 2)
					{
						Vector2 targetPos = npc.position + new Vector2(npc.width * .5f - 200 * projectile.ai[0], 230) - projectile.Center;
						projectile.rotation = targetPos.ToRotation() + 1.57f;

						projectile.velocity.Y *= .95f;
						projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X + (.1f * -projectile.ai[0]), -8, 8);

						if (projectile.Center.X < owner.Center.X - 500 || projectile.Center.X > owner.Center.X + 500)
						{
							projectile.localAI[1] = 3;
							projectile.netUpdate = true;
							projectile.velocity = Vector2.Normalize(npc.Center - projectile.Center) * 17;
						}
					}
					else if (projectile.localAI[1] == 3 && (
						(projectile.velocity.X > 0 && projectile.Center.X > npc.Center.X) ||
						(projectile.velocity.X < 0 && projectile.Center.X < npc.Center.X)))
					{
						projectile.localAI[1] = 0;
						this.attacking = false;
					}
				}
				// Ranged attack for both the laser and cannon.
				else
				{
					// Shooting
					int cooldown = primeType == NPCID.PrimeLaser ? 60 : 180;
					Vector2 dirToNPC = npc.Center - projectile.Center;
					projectile.rotation = dirToNPC.ToRotation() - 1.57f;

					dirToNPC.Normalize();
					if (projectile.localAI[1]++ >= cooldown && Main.netMode != NetmodeID.MultiplayerClient)
					{
						int newProjType = primeType == NPCID.PrimeLaser ? ProjectileID.GreenLaser : ProjectileID.BombSkeletronPrime;

						int newProj = Projectile.NewProjectile(projectile.Center, dirToNPC * 8, newProjType, projectile.damage, projectile.knockBack, projectile.owner);
						Main.projectile[newProj].hostile = false;
						Main.projectile[newProj].friendly = true;
						Main.projectile[newProj].netUpdate = true;

						projectile.localAI[1] = 0;
					}

					// Movement.
					float yLength = Math.Min(Math.Abs(projectile.position.Y - targetHoverPosition.Y) * damping, yAcceleration);
					float xLength = Math.Min(Math.Abs(projectile.position.X - targetHoverPosition.X) * damping, xAcceleration);

					// Movement.
					if (projectile.position.Y > targetHoverPosition.Y)
					{
						if (projectile.velocity.Y > 0)
							projectile.velocity.Y *= .96f;
						projectile.velocity.Y -= yLength;
					}
					else if (projectile.position.Y < targetHoverPosition.Y)
					{
						if (projectile.velocity.Y < 0)
							projectile.velocity.Y *= .96f;
						projectile.velocity.Y += yLength;
					}

					if (projectile.Center.X > targetHoverPosition.X)
					{
						if (projectile.velocity.X > 0)
							projectile.velocity.X *= .96f;
						projectile.velocity.X -= xLength;
					}
					else if (projectile.Center.X < targetHoverPosition.X)
					{
						if (projectile.velocity.X < 0)
							projectile.velocity.X *= .96f;
						projectile.velocity.X += xLength;
					}

					// Clamp velocity.
					projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -6, 6);
					projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -8, 8);
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Player owner = Main.player[projectile.owner];

			Vector2 armOrigin = new Vector2(Main.boneArm2Texture.Width * .5f, Main.boneArm2Texture.Height * .5f);
			Vector2 armStartPos = new Vector2(projectile.Center.X - 5 * projectile.ai[0], projectile.position.Y + 20f);

			for (int i = 0; i < 2; ++i)
			{
				Vector2 direction = new Vector2(owner.position.X + owner.width * .5f, owner.position.Y) - armStartPos;
				float length;
				if (i == 0)
				{
					direction.X -= 200 * projectile.scale * projectile.ai[0];
					direction.Y += 60 * projectile.scale * projectile.ai[1];
					length = direction.Length();
					length = (92 * (projectile.scale + .1f)) / length;
				}
				else
				{
					direction.X -= 50 * projectile.ai[0];
					length = direction.Length();
					length = (60 * (projectile.scale + .1f)) / length;
				}
				armStartPos += (direction * length) * projectile.scale;

				float rotation = direction.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor((int)armStartPos.X / 16, (int)armStartPos.Y / 16);
				spriteBatch.Draw(Main.boneArm2Texture, armStartPos - Main.screenPosition, null, color, rotation, armOrigin, projectile.scale, SpriteEffects.None, 0);

				if (i == 0)
					armStartPos += direction * length / 2;
				else if (projectile.active)
				{
					armStartPos += direction * length - new Vector2(16, 6);
					Dust d = Main.dust[Dust.NewDust(armStartPos, 30, 10, DustID.Smoke, direction.X * .02f, direction.Y * .02f, 0, default, .6f)];
					d.noGravity = true;
				}
			}

			int projFrames = 1;
			if (primeType == NPCID.PrimeSaw || primeType == NPCID.PrimeVice)
				projFrames = 2;

			if (projFrames != 1 && projectile.frameCounter++ >= 15)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % projFrames;
			}

			Main.instance.LoadNPC(primeType);
			Texture2D projTexture = Main.npcTexture[primeType];
			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, (projTexture.Height / projFrames) * .5f);

			SpriteEffects effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, Utils.Frame(projTexture, 1, projFrames, 0, projectile.frame),
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, effects, 0);

			return (false);
		}

		#region Networking Section

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
			writer.Write(this.target);
			writer.Write(this.primeType);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
			this.target = reader.ReadInt32();
			this.primeType = reader.ReadInt32();
		}

		#endregion
	}

	/*internal sealed class SkeletronPrimeSoulProj_Chainsaw
	{
		public override string Texture => "Terraria/NPC_" + NPCID.PrimeSaw;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
		}

		public override bool PreAI()
		{
			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}
	}

	internal sealed class SkeletronPrimeSoulProj_Vice
	{
		public override string Texture => "Terraria/NPC_" + NPCID.PrimeVice;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
		}

		public override bool PreAI()
		{
			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}
	}*/
}
