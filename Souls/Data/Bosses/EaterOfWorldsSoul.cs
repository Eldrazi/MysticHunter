using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System.IO;
using System;

namespace MysticHunter.Souls.Data.Bosses
{
	public class EaterOfWorldsSoul : PreHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.EaterofWorldsHead;
		public override string soulDescription => "Summons a hungry Eater of Worlds.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<EaterOfWorlsSoulProj>())
					Main.projectile[i].Kill();

			int amount = 3;
			if (stack >= 5)
				amount += 2;
			if (stack >= 9)
				amount += 2;

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 4;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<EaterOfWorlsSoulProj>(), 25 + stack, .2f, p.whoAmI, amount);
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail };
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

	public class EaterOfWorlsSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_13";

		private int bodyLength => (int)projectile.ai[0];

		private int target;
		private BodyPart[] bodyParts;

		private readonly float minAttackDistance = 60;
		private readonly float maxTargetingDistance = 320;

		private readonly float speed = 6;
		private readonly float acceleration = .09f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eater of Worlds");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 36;

			projectile.scale = .4f;
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

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoul?.soulNPC == NPCID.EaterofWorldsHead)
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
					else
					{
						targetPosition = Main.npc[target].Center;

						Vector2 desiredDirection = Vector2.Normalize(targetPosition - projectile.Center);
						Vector2 normalizedVelocity = Vector2.Normalize(projectile.velocity);

						if (desiredDirection.Length() <= minAttackDistance && Vector2.Dot(desiredDirection, normalizedVelocity) >= .8f)
						{
							if (projectile.localAI[1]++ >= 6 && Main.netMode != NetmodeID.MultiplayerClient)
							{
								projectile.localAI[1] = 0;
								Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileType<EaterOfWorldSoulProjFlame>(), projectile.damage, .1f, owner.whoAmI);
							}
						}
					}
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

		public override bool CanDamage() => false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D bodyTexture = GetTexture("Terraria/NPC_14");
			Texture2D tailTexture = GetTexture("Terraria/NPC_15");

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

		#region Projectile Specific Logic

		private void UpdateBodyParts()
		{
			Vector2 centerModifier = new Vector2(projectile.width * .5f, projectile.height * .5f);
			for (int i = 0; i < bodyParts.Length; ++i)
			{
				Vector2 otherCenter = projectile.position + centerModifier;
				if (i != 0)
					otherCenter = bodyParts[i - 1].position + centerModifier;

				Vector2 directionToOtherPart = otherCenter - (bodyParts[i].position + centerModifier);
				bodyParts[i].rotation = directionToOtherPart.ToRotation() + MathHelper.PiOver2;

				float length = directionToOtherPart.Length();
				int lengthModifier = (int)(32 * projectile.scale);

				length = (length - lengthModifier) / length;
				directionToOtherPart *= length;
				bodyParts[i].position += directionToOtherPart;
			}
		}

		#endregion
	}

	public class EaterOfWorldSoulProjFlame : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_0";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Flame");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 6;

			projectile.penetrate = 3;
			projectile.extraUpdates = 2;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			// Projectile kill conditions.
			if (projectile.timeLeft > 40)
				projectile.timeLeft = 40;
			if (projectile.wet && !projectile.lavaWet)
				projectile.Kill();

			if (projectile.ai[0] >= 8)
			{
				float size = 1;
				if (projectile.ai[0] <= 10)
					size = (projectile.ai[0] - 7) * .25f;

				projectile.ai[0]++;

				int dustID = 75;

				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, dustID, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100)];
				if (Main.rand.Next(3) != 0)
				{
					d.noGravity = true;
					d.scale *= 3f;
					d.velocity *= 2;
				}
				d.velocity *= 1.2f;
				d.scale *= size;
			}
			else
				projectile.ai[0]++;

			projectile.rotation += 0.3f * projectile.direction;
			return (false);
		}
	}
}
