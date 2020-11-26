using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class SkeletronSoul : PreHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.SkeletronHead;
		public override string soulDescription => "Summon protective arms.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<SkeletronSoulProj>())
					Main.projectile[i].Kill();

			int damage = 50;

			int amount = 1;
			if (stack >= 5)
				amount++;

			for (int i = 0; i < amount; ++i)
			{
				int xHand = (i % 2 == 0 ? -1 : 1);

				Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<SkeletronSoulProj>(), damage, .2f, p.whoAmI, xHand);
			}
			return (true);
		}
	}

	public class SkeletronSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_36";

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
				position.Y += 130 * projectile.scale;

				return (position);
			}
		}

		private int target;

		private readonly float damping = .001f;
		private readonly float yAcceleration = .12f, xAcceleration = .2f;
		private readonly float maxTargetingDistance = 320;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skeletal Hand");
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

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoul?.soulNPC == NPCID.SkeletronHead)
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
				Vector2 ownerPos = owner.position + new Vector2(owner.width * .5f, 0);
				Vector2 dir = ownerPos + new Vector2(-200 * projectile.ai[0], 130) - projectile.Center;
				projectile.rotation = dir.ToRotation() + MathHelper.PiOver2;

				// Finding target.
				if (projectile.ai[1]++ >= 320)
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
						projectile.ai[1] = 0;
						this.attacking = true;
					}
				}
			}
			else
			{
				NPC npc = Main.npc[target];

				if (!npc.active)
				{
					projectile.ai[1] = 0;
					this.attacking = false;
				}

				if (projectile.ai[1] == 0)
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
						projectile.ai[1] = 1;
						projectile.netUpdate = true;
						projectile.velocity = Vector2.Normalize(npc.Center - projectile.Center) * 18;
					}
				}
				else if (projectile.ai[1] == 1)
				{
					if (projectile.position.Y > npc.position.Y || projectile.velocity.Y < 0f)
						projectile.ai[1] = 2;
				}
				else if (projectile.ai[1] == 2)
				{
					Vector2 targetPos = npc.position + new Vector2(npc.width * .5f - 200 * projectile.ai[0], 230) - projectile.Center;
					projectile.rotation = targetPos.ToRotation() + 1.57f;

					projectile.velocity.Y *= .95f;
					projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X + (.1f * -projectile.ai[0]), -8, 8);

					if (projectile.Center.X < owner.Center.X - 500 || projectile.Center.X > owner.Center.X + 500)
					{
						projectile.ai[1] = 3;
						projectile.netUpdate = true;
						projectile.velocity = Vector2.Normalize(npc.Center - projectile.Center) * 17;
					}
				}
				else if (projectile.ai[1] == 3 && (
					(projectile.velocity.X > 0 && projectile.Center.X > npc.Center.X) || 
					(projectile.velocity.X < 0 && projectile.Center.X < npc.Center.X)))
				{
					projectile.ai[1] = 0;
					this.attacking = false;
				}
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Player owner = Main.player[projectile.owner];

			Vector2 armOrigin = new Vector2(Main.boneArmTexture.Width * .5f, Main.boneArmTexture.Height * .5f);
			Vector2 armStartPos = new Vector2(projectile.Center.X - 5 * projectile.ai[0], projectile.position.Y + 20f);

			for (int i = 0; i < 2; ++i)
			{
				Vector2 direction = new Vector2(owner.position.X + owner.width * .5f, owner.position.Y) - armStartPos;
				float length;
				if (i == 0)
				{
					direction.X -= 200 * projectile.scale * projectile.ai[0];
					direction.Y += 60;
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
				spriteBatch.Draw(Main.boneArmTexture, armStartPos - Main.screenPosition, null, color, rotation, armOrigin, projectile.scale, SpriteEffects.None, 0);

				if (i == 0)
					armStartPos += direction * length / 2;
				else if (projectile.active)
				{
					armStartPos += direction * length - new Vector2(16, 6);
					Dust d = Main.dust[Dust.NewDust(armStartPos, 30, 10, DustID.Smoke, direction.X * .02f, direction.Y * .02f, 0, default, .6f)];
					d.noGravity = true;
				}
			}

			Texture2D projTexture = GetTexture(Texture);
			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, projTexture.Height * .5f);

			SpriteEffects effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, null,
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, effects, 0);

			return (false);
		}

		#region Networking Section

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(this.target);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			this.target = reader.ReadInt32();
		}

		#endregion
	}
}
