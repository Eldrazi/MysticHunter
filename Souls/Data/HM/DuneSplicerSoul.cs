using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System.IO;
using System;

namespace MysticHunter.Souls.Data.HM
{
	public class DuneSplicerSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DuneSplicerHead;
		public override string soulDescription => "Summons a friendly Dune Splicer.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 55 + 5 * stack;

			int amount = 3;
			if (stack >= 5)
				amount += 2;
			if (stack >= 9)
				amount += 2;

			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<DuneSplicerSoulProj>())
					Main.projectile[i].Kill();
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 4;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<DuneSplicerSoulProj>(), damage, .2f, p.whoAmI, amount);
			return (true);
		}
	}

	public class DuneSplicerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_510";

		private int bodySize = 36;
		private int bodyLength => (int)projectile.ai[0];

		private int target;
		private BodyPart[] bodyParts;

		private readonly float maxTargetingDistance = 320;

		private readonly float speed = 6;
		private readonly float acceleration = .07f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dune Splicer");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 24;

			projectile.scale = .6f;
			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			this.target = 255;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead || owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Blue].soulNPC == NPCID.DuneSplicerHead)
				projectile.timeLeft = 2;

			if (projectile.ai[1] == 0)
			{
				projectile.ai[1] = 1;
				bodyParts = new BodyPart[bodyLength];
				for (int i = 0; i < bodyParts.Length; ++i)
					bodyParts[i] = new BodyPart(projectile.position);
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
						this.target = 255;
				}

				// Calculate desired velocity towards target.
				Vector2 projectileCenter = new Vector2((int)(projectile.Center.X / 16) * 16, (int)(projectile.Center.Y / 16) * 16);
				targetPosition = new Vector2((int)(targetPosition.X / 16) * 16, (int)(targetPosition.Y / 16) * 16);

				targetPosition = Vector2.Normalize(targetPosition - projectileCenter) * speed;

				// Update velocity.
				if ((projectile.velocity.X > 0 && targetPosition.X > 0) || (projectile.velocity.X < 0 && targetPosition.X < 0) ||
					(projectile.velocity.Y > 0 && targetPosition.Y > 0) || (projectile.velocity.Y < 0 && targetPosition.Y < 0))
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D bodyTexture = GetTexture("Terraria/NPC_" + NPCID.DuneSplicerBody);
			Texture2D tailTexture = GetTexture("Terraria/NPC_" + NPCID.DuneSplicerTail);

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
			
			Texture2D projTexture = GetTexture(Texture);

			Vector2 projOrigin = new Vector2(currentTexture.Width * .5f, currentTexture.Height * .5f);

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

		public override void Kill(int timeLeft)
		{
			SpawnKillDust(projectile.position);
			for (int i = 0; i < bodyLength; ++i)
				SpawnKillDust(bodyParts[i].position);
		}

		private void SpawnKillDust(Vector2 position)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(position, projectile.width, projectile.height, DustID.Sandstorm, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		#region Projectile Specific Logic

		private void UpdateBodyParts()
		{
			Vector2 centerModifier = new Vector2(projectile.width * .5f, projectile.height * .5f);
			for (int i = 0; i < bodyParts.Length; ++i)
			{
				int size = this.bodySize - 8;
				Vector2 otherCenter = projectile.position + centerModifier;
				if (i != 0)
				{
					size = this.bodySize;
					otherCenter = bodyParts[i - 1].position + centerModifier;
				}

				Vector2 directionToOtherPart = otherCenter - (bodyParts[i].position + centerModifier);
				bodyParts[i].rotation = directionToOtherPart.ToRotation() + MathHelper.PiOver2;

				float length = directionToOtherPart.Length();
				int lengthModifier = (int)(size * projectile.scale);

				length = (length - lengthModifier) / length;
				directionToOtherPart *= length;
				bodyParts[i].position += directionToOtherPart;
			}
		}

		#endregion
	}
}
