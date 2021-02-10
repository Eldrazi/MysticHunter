﻿#region Using directives

using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;
using MysticHunter.Common.Utils;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class CrawltipedeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.SolarCrawltipedeHead;
		public override string soulDescription => "Grow a daybreak inflicting tail.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<CrawltipedeSoul_Proj>())
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			int damage = 180 + 20 * stack;

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<CrawltipedeSoul_Proj>(), damage, .7f, p.whoAmI, -1);
			return (true);
		}
	}

	public class CrawltipedeSoul_Proj : ModProjectile
	{
		private readonly int tailParts= 18;
		private readonly float tailScale = .5f;
		private readonly int tailPartLength = 24;
		private int TailLength
		{
			get { return (int)(tailPartLength * tailScale) * tailParts; }
		}

		private int randomIdleRotationTimer = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crawling Tail");
		}
		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 30;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			projectile.manualDirectionChange = true;

			projectile.scale = .8f;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.SolarCrawltipedeHead)
			{
				projectile.timeLeft = 2;
			}

			// Projectile state management.
			float maxSpeed = .25f;
			Vector2 targetPosition = Vector2.Zero;

			// Projectile state: Idle.
			if (projectile.ai[0] == -1)
			{
				maxSpeed = 1;

				// Default position right above the player's head.
				targetPosition.X = owner.position.X + (owner.width / 2) * projectile.scale;
				targetPosition.Y = owner.position.Y - 20;

				if (++randomIdleRotationTimer >= 60)
				{
					randomIdleRotationTimer = 0;
					projectile.localAI[0] = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
				}

				//projectile.rotation = (float)(owner.direction == -1 ? Math.PI : 0) - MathHelper.PiOver2;
				projectile.rotation = MathHelper.Lerp(projectile.rotation, (owner.direction == -1 ? MathHelper.Pi : 0) + projectile.localAI[0] - MathHelper.PiOver2, 0.05f);

				if (projectile.direction != -owner.direction)
				{
					projectile.direction = -owner.direction;
					projectile.rotation += MathHelper.Pi * projectile.direction;
				}

				// Projectile target acquisition.
				if (++projectile.ai[1] >= 120)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						if (Main.npc[i].CanBeChasedBy(projectile) && Vector2.Distance(owner.Center, Main.npc[i].Center) <= TailLength)
						{
							projectile.ai[0] = i;
							projectile.ai[1] = 0;
							projectile.netUpdate = true;
							break;
						}
					}
				}
			}
			// Projectile state: target acquired.
			else
			{
				// Check if target NPC is still alive and in-range.
				NPC target = Main.npc[(int)projectile.ai[0]];
				if (Main.myPlayer == projectile.owner && (!target.active || Vector2.Distance(owner.Center, target.Center) > TailLength))
				{
					projectile.ai[0] = -1;
					projectile.netUpdate = true;
					return (false);
				}

				targetPosition = target.Center;
				projectile.rotation = (targetPosition - projectile.Center).ToRotation() - MathHelper.PiOver2;
			}

			// Teleport in case there's too much distance between projectile and owner.
			if (Vector2.Distance(projectile.Center, owner.Center) > TailLength * 1.5f)
			{
				Vector2 teleportDir = Vector2.Normalize(owner.Center - projectile.Center);
				projectile.Center = owner.Center + teleportDir * (TailLength * .75f);
				projectile.netUpdate = true;
			}

			targetPosition -= projectile.Center;
			targetPosition *= (maxSpeed / TailLength) * targetPosition.Length();
			projectile.velocity = targetPosition;

			projectile.spriteDirection = -projectile.direction;

			return (false);
		}

		public override bool CanDamage()
			=> projectile.ai[0] != -1;

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			// If the projectile is not in idle state.
			if (projectile.ai[0] != -1)
			{
				projectile.ai[0] = -1;
				projectile.netUpdate = true;
			}

			target.AddBuff(BuffID.Daybreak, 180);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
			=> drawCacheProjsBehindNPCsAndTiles.Add(index);

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Player owner = Main.player[projectile.owner];

			Texture2D tailPartTex = ModContent.GetTexture("MysticHunter/Souls/Data/Event/LunarEvents/CrawltipedeSoul_Proj_Chain");

			Vector2 origin = new Vector2(tailPartTex.Width / 2, tailPartTex.Height / 2);

			float startRot = (projectile.rotation + MathHelper.Pi) % MathHelper.TwoPi;

			Vector2 startPos = projectile.Center - ((startRot - MathHelper.PiOver2).ToRotationVector2() * tailPartLength * tailScale);
			Vector2 endPos = owner.Center + new Vector2(0, 10);
			Rectangle drawRect = tailPartTex.Bounds;

			int maxLen = 1;
			int dir = -1;
			if (startRot > MathHelper.Pi)
			{
				dir = 1;
			}

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
				float rotDiff = (float)((rotOpt2 - startRot + 2.5f * Math.PI) % (2 * Math.PI) - Math.PI);
				if (rotDiff <= rotOpt1 && rotDiff >= -rotOpt1)
					startRot += rotDiff;
				else
					startRot += currentRotStep * dir;

				startPos -= ((startRot - MathHelper.PiOver2).ToRotationVector2() * tailPartLength) * tailScale;

				float distanceToEndpoint = Vector2.Distance(startPos, endPos);
				if (i == maxLen - 1 && maxLen < tailParts && distanceToEndpoint > (tailPartLength / 2) * tailScale)
					maxLen++;

				// Check to see if the draw rectangle needs to be shortened.
				if (Vector2.Distance(startPos, endPos) < distanceToEndpoint)
					drawRect.Height = (int)distanceToEndpoint;
			}

			return (this.DrawAroundOrigin(spriteBatch, lightColor));
		}
	}
}
