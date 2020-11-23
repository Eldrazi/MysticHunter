using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System.Collections.Generic;

namespace MysticHunter.Souls.Data.HM
{
	public class WorldFeederSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.SeekerHead;
		public override string soulDescription => "Grow a tail that fires cursed flames.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<WorldFeederSoulProj>())
					Main.projectile[i].Kill();
			}
			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<WorldFeederSoulProj>(), 80 + (2 * stack), .2f, p.whoAmI, -1);
			return (true);
		}
	}

	public class WorldFeederSoulProj : ModProjectile
	{
		private readonly int tailParts= 10;
		private readonly float tailScale = .5f;
		private readonly int tailPartLength = 24;
		private int TailLength
		{
			get { return (int)(tailPartLength * tailScale) * tailParts; }
		}

		private readonly float targetingDistance = 300;

		private const float minFalseTargetingRot = MathHelper.PiOver4 * .5f;
		private const float maxFalseTargetingRot = (float)Math.PI - MathHelper.PiOver4 * .5f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Feeder Tail");
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 20;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			projectile.scale = .8f;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Blue].soulNPC == NPCID.SeekerHead)
				projectile.timeLeft = 2;

			// Projectile state management.
			float maxSpeed = .5f;
			Vector2 targetPosition = Vector2.Zero;

			// Projectile state: Idle.
			if (projectile.ai[0] == -1)
			{
				maxSpeed = 1;

				// Default position right above the player's head.
				targetPosition.X = owner.position.X + (owner.width / 2) * projectile.scale;
				targetPosition.Y = owner.position.Y - 20;

				projectile.rotation = MathHelper.PiOver2 + (float)(owner.direction == 1 ? Math.PI : 0);

				// Projectile target acquisition.
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy(projectile) && Vector2.Distance(projectile.Center, Main.npc[i].Center) <= targetingDistance &&
						Collision.CanHitLine(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						float rotationTowards = (Main.npc[i].Center - projectile.Center).ToRotation();
						if (rotationTowards >= minFalseTargetingRot && rotationTowards <= maxFalseTargetingRot)
							continue;
						projectile.ai[0] = i;
						projectile.ai[1] = 0;
						projectile.netUpdate = true;
						break;
					}
				}
			}
			// Projectile state: target acquired.
			else
			{
				// Check if target NPC is still alive and in-range.
				NPC target = Main.npc[(int)projectile.ai[0]];
				float rotationTowards = (target.Center - projectile.Center).ToRotation();
				if (Main.myPlayer == projectile.owner)
				{ 
					if ((!target.active || Vector2.Distance(owner.Center, target.Center) > targetingDistance) ||
					(rotationTowards >= minFalseTargetingRot && rotationTowards <= maxFalseTargetingRot))
					{
						projectile.ai[0] = -1;
						projectile.netUpdate = true;
					}
					else
					{
						if (projectile.ai[1]++ >= 120)
						{
							projectile.ai[1] = 0;
							Projectile.NewProjectile(projectile.Center, rotationTowards.ToRotationVector2() * 6f, ProjectileID.CursedFlameFriendly, projectile.damage, .1f, projectile.owner);
						}
					}
				}

				targetPosition.X = owner.position.X + (owner.width / 2) * projectile.scale;
				targetPosition.Y = owner.position.Y - 20;
				projectile.rotation = rotationTowards - MathHelper.PiOver2;
			}

			// Teleport in case there's too much distance between projectile and owner.
			if (Vector2.Distance(projectile.Center, owner.Center) > TailLength * 1.5f)
			{
				Vector2 teleportDir = Vector2.Normalize(owner.Center - projectile.Center);
				projectile.Center = owner.Center + teleportDir * (TailLength * .75f);
				projectile.netUpdate = true;
			}

			targetPosition = (targetPosition - projectile.Center);
			targetPosition *= (maxSpeed / TailLength) * targetPosition.Length();
			projectile.velocity = targetPosition;
			return (false);
		}

		public override bool CanDamage() => false;

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
			=> drawCacheProjsBehindNPCsAndTiles.Add(index);

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Player player = Main.player[projectile.owner];

			Texture2D tailPartTex = GetTexture("MysticHunter/Souls/Data/HM/WorldFeederSoulProj_Chain");

			Vector2 origin = new Vector2(tailPartTex.Width / 2, tailPartTex.Height / 2);

			float startRot = (projectile.rotation + (float)Math.PI) % MathHelper.TwoPi;

			Vector2 startPos = projectile.Center - ((startRot - MathHelper.PiOver2).ToRotationVector2() * tailPartLength * tailScale);
			Vector2 endPos = player.Center + new Vector2(0, 10);
			Rectangle drawRect = tailPartTex.Bounds;

			int maxLen = 1;
			int dir = (startPos.X > player.Center.X + tailPartLength * Math.Sign(Math.Cos(startRot - MathHelper.PiOver2))) ? 1 : -1;
			for (int i = 0; i < maxLen; ++i)
			{
				// Draw the current tail part/piece.
				spriteBatch.Draw(tailPartTex, startPos - Main.screenPosition, drawRect, lightColor, startRot + (float)Math.PI, origin, tailScale, SpriteEffects.None, 0);

				// Calculate two rotational options:
				// 1. A semi-preset rotation which is dependant on the index of the current tail piece.
				// 2. A calculated rotation, based on the position of the last tail piece in relation to the end-piece of the tail.
				float rotOpt1 = (float)Math.PI / Math.Abs(8 - i);
				float rotOpt2 = (endPos - startPos).ToRotation();

				// Check which rotation we're going to use by checking if the desired angle (opt2) is within rotation range for this tail part.
				float currentRotStep = rotOpt1;
				float rotDiff = (float)((rotOpt2 - startRot + 2.5f * Math.PI) % (2*Math.PI) - Math.PI);
				if (rotDiff <= rotOpt1 && rotDiff >= -rotOpt1)
					startRot += rotDiff;
				else
					startRot += currentRotStep * dir;

				startPos -= ((startRot - MathHelper.PiOver2).ToRotationVector2() * tailPartLength) * tailScale;

				float distanceToEndpoint = Vector2.Distance(startPos, endPos);
				if (i == maxLen - 1 && maxLen < tailParts && distanceToEndpoint > (tailPartLength/2) * tailScale)
					maxLen++;

				// Check to see if the draw rectangle needs to be shortened.
				if (Vector2.Distance(startPos, endPos) < distanceToEndpoint)
					drawRect.Height = (int)distanceToEndpoint;
			}

			return (true);
		}
	}
}
