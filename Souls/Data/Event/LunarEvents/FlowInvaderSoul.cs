#region Using directives

using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class FlowInvaderSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.StardustJellyfishBig;
		public override string soulDescription => "Summons a friendly Milkyway Weaver.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 40;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<FloInvaderSoulProj>())
				{
					Main.projectile[i].Kill();
				}
			}

			int damage = 210 + 10 * stack;
			int amount = 1;
			if (stack >= 5)
				amount += 1;
			if (stack >= 9)
				amount++;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6;
			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(p.Center, velocity.RotatedByRandom(MathHelper.PiOver4), ModContent.ProjectileType<FloInvaderSoulProj>(), damage, 0.5f, p.whoAmI);
			}
			return (true);
		}
	}

	internal class BodyPart
	{
		public float rotation;
		public Vector2 position;

		public BodyPart(Vector2 position)
		{
			this.rotation = 0;
			this.position = position;
		}
	}

	public class FloInvaderSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.StardustWormHead;

		private int bodyLength => 6;

		private int target;
		private BodyPart[] bodyParts;

		private readonly float maxTargetingDistance = 420;

		private readonly float speed = 6;
		private readonly float acceleration = .09f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Milkyway Weaver");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.StardustWormHead];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0f;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			this.target = 255;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.StardustJellyfishBig)
			{
				projectile.timeLeft = 2;
			}

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == projectile.type)
				{
					Vector2 directionToOther = projectile.Center - Main.projectile[i].Center;
					if (directionToOther.HasNaNs() || directionToOther == Vector2.Zero)
					{
						directionToOther = -Vector2.UnitY;
					}

					if (directionToOther.Length() <= projectile.width * 2)
					{
						projectile.velocity += Vector2.Normalize(directionToOther) * 0.05f;
					}
				}
			}

			if (projectile.ai[1] == 0)
			{
				projectile.ai[1] = 1;
				bodyParts = new BodyPart[bodyLength];
				for (int i = 0; i < bodyParts.Length; ++i)
				{
					bodyParts[i] = new BodyPart(projectile.position);
				}
			}
			else
			{
				Vector2 targetPosition = owner.Center;

				// Searching for a target.
				if (this.target == 255)
				{
					float distance = maxTargetingDistance;
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						if (Main.npc[i].CanBeChasedBy(projectile))
						{
							float d = Vector2.Distance(Main.npc[i].Center, owner.Center);
							if (d <= distance)
							{
								target = i;
								distance = d;
								projectile.netUpdate = true;
							}
						}
					}
				}
				else
				{
					if (!Main.npc[target].active || Vector2.Distance(Main.npc[target].Center, owner.Center) > maxTargetingDistance * 2)
					{
						this.target = 255;
					}
					else
					{
						targetPosition = Main.npc[target].Center;
					}
				}

				// Calculate desired velocity towards target.
				Vector2 projectileCenter = new Vector2((int)(projectile.Center.X / 16) * 16, (int)(projectile.Center.Y / 16) * 16);
				targetPosition = new Vector2((int)(targetPosition.X / 16) * 16, (int)(targetPosition.Y / 16) * 16);

				targetPosition = Vector2.Normalize(targetPosition - projectileCenter) * speed;
				if (this.target != 255)
				{
					targetPosition *= 2;
				}
				else
				{
					if ((projectile.position - owner.position).Length() >= 800)
					{
						DustEffect();
						projectile.position = owner.Center + new Vector2(Main.rand.NextFloat(-1, 1) * 60, Main.rand.NextFloat(-1, 1) * 60);
						DustEffect();
					}
				}

				// Update velocity.
				if (Math.Sign(projectile.velocity.X) == Math.Sign(targetPosition.X) ||
					Math.Sign(projectile.velocity.Y) == Math.Sign(targetPosition.Y))
				{
					if (projectile.velocity.X < targetPosition.X)
						projectile.velocity.X += acceleration;
					else if (projectile.velocity.X > targetPosition.X)
						projectile.velocity.X -= acceleration;

					if (projectile.velocity.Y < targetPosition.Y)
						projectile.velocity.Y += acceleration;
					else if (projectile.velocity.Y > targetPosition.Y)
						projectile.velocity.Y -= acceleration;

					if (Math.Abs(targetPosition.Y) < speed * .2f && ((projectile.velocity.X > 0 && targetPosition.X < 0) || (projectile.velocity.X < 0 && targetPosition.X > 0)))
					{
						if (projectile.velocity.Y > 0)
							projectile.velocity.Y += acceleration * 2f;
						else
							projectile.velocity.Y -= acceleration * 2;
					}
					if (Math.Abs(targetPosition.X) < speed * .2 && ((projectile.velocity.Y > 0 && targetPosition.Y < 0) || (projectile.velocity.Y < 0 && targetPosition.Y > 0)))
					{
						if (projectile.velocity.X > 0)
							projectile.velocity.X += acceleration * .2f;
						else
							projectile.velocity.X -= acceleration * .2f;
					}
				}
				else if (Math.Abs(targetPosition.X) > Math.Abs(targetPosition.Y))
				{
					if (projectile.velocity.X < targetPosition.X)
						projectile.velocity.X += acceleration * 1.1f;
					else if (projectile.velocity.X > targetPosition.X)
						projectile.velocity.X -= acceleration * 1.1f;

					if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < speed * .5f)
					{
						if (projectile.velocity.Y > 0)
							projectile.velocity.Y += acceleration;
						else
							projectile.velocity.Y -= acceleration;
					}
				}
				else
				{
					if (projectile.velocity.Y < targetPosition.Y)
						projectile.velocity.Y += acceleration * 1.1f;
					else if (projectile.velocity.Y > targetPosition.Y)
						projectile.velocity.Y -= acceleration * 1.1f;

					if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < speed * .5f)
					{
						if (projectile.velocity.X > 0)
							projectile.velocity.X += acceleration;
						else
							projectile.velocity.X -= acceleration;
					}
				}

				UpdateBodyParts();
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			return (false);
		}

		public override void Kill(int timeLeft)
			=> DustEffect();

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D bodyTexture = ModContent.GetTexture("Terraria/NPC_" + NPCID.StardustWormBody);
			Texture2D tailTexture = ModContent.GetTexture("Terraria/NPC_" + NPCID.StardustWormTail);

			Texture2D currentTexture = bodyTexture;
			Vector2 origin = new Vector2(currentTexture.Width * .5f, currentTexture.Height * .5f);
			for (int i = 0; i < bodyParts.Length; ++i)
			{
				// Draw body parts.
				if (i == bodyParts.Length - 1)
					currentTexture = tailTexture;

				spriteBatch.Draw(currentTexture, bodyParts[i].position + new Vector2(projectile.width * .5f, projectile.height * .5f) - Main.screenPosition, null,
					Lighting.GetColor((int)bodyParts[i].position.X / 16, (int)bodyParts[i].position.Y / 16),
					bodyParts[i].rotation, origin, projectile.scale, SpriteEffects.None, 0);
			}
			
			Texture2D projTexture = Main.projectileTexture[projectile.type];

			Vector2 projOrigin = new Vector2(currentTexture.Width * 0.5f, currentTexture.Height * 0.5f);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, null,
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);


			return (false);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(target);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			target = reader.ReadInt32();
		}

		#region Projectile Specific Logic

		private void UpdateBodyParts()
		{
			Vector2 centerModifier = new Vector2(projectile.width * .5f, projectile.height * .5f);
			for (int i = 0; i < bodyParts.Length; ++i)
			{
				Vector2 otherCenter = projectile.position + centerModifier;
				if (i != 0)
				{
					otherCenter = bodyParts[i - 1].position + centerModifier;
				}

				Vector2 directionToOtherPart = otherCenter - (bodyParts[i].position + centerModifier);
				bodyParts[i].rotation = directionToOtherPart.ToRotation() + MathHelper.PiOver2;

				float length = directionToOtherPart.Length();
				int lengthModifier = (int)(18 * projectile.scale);

				if (i == 0)
				{
					lengthModifier -= 10;
				}

				length = (length - lengthModifier) / length;
				directionToOtherPart *= length;
				bodyParts[i].position += directionToOtherPart;
			}
		}

		private void DustEffect()
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0, 0, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		#endregion
	}
}
