using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System.Collections.Generic;

namespace MysticHunter.Souls.Data
{
	public class GiantWormSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GiantWormHead;
		public string soulDescription => "Grow a whip like tail.";

		public short cooldown => 600;

		public SoulType soulType => SoulType.Blue;

		public short ManaCost(Player p, short stack) => 10;
		public bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<GiantWormSoulProj>())
					Main.projectile[i].Kill();
			}

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<GiantWormSoulProj>(), 10 + (1 * stack), .05f + .01f * stack, p.whoAmI, -1);
			return (true);
		}
	}

	public class GiantWormSoulProj : ModProjectile
	{
		private readonly int tailParts= 8;
		private readonly float tailScale = .5f;
		private readonly int tailPartLength = 20;
		private int TailLength
		{
			get { return (int)(tailPartLength * tailScale) * tailParts; }
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tail");
		}
		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 30;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = false;

			projectile.scale = .8f;
		}

		public override bool PreAI()
		{
			Player player = Main.player[projectile.owner];
			
			// Check if the projectile should still be alive.
			if (player.dead || player.GetModPlayer<SoulPlayer>().souls[1] == null || player.GetModPlayer<SoulPlayer>().souls[1].soulNPC != NPCID.GiantWormHead)
				projectile.Kill();

			// Projectile state management.
			float maxSpeed = .5f;
			Vector2 targetPosition = Vector2.Zero;

			// Projectile state: Idle.
			if (projectile.ai[0] == -1)
			{
				maxSpeed = 1;

				// Default position right above the player's head.
				targetPosition.X = player.position.X + (player.width / 2) * projectile.scale;
				targetPosition.Y = player.position.Y - 20;

				projectile.rotation = MathHelper.PiOver2 + (float)(player.direction == 1 ? Math.PI : 0);

				// Projectile target acquisition.
				if (projectile.ai[1]++ >= 120)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						if (Main.npc[i].active && Vector2.Distance(player.Center, Main.npc[i].Center) <= TailLength)
						{
							projectile.ai[0] = i;
							projectile.ai[1] = 0;
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
				if (!target.active || Vector2.Distance(player.Center, target.Center) > TailLength)
					projectile.ai[0] = -1;

				targetPosition = target.Center;
				projectile.rotation = MathHelper.PiOver2 + (float)(targetPosition.X > projectile.Center.X ? Math.PI : 0);
			}

			targetPosition = (targetPosition - projectile.Center);
			targetPosition *= (maxSpeed / TailLength) * targetPosition.Length();
			projectile.velocity = targetPosition;

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			// If the projectile is not in idle state.
			if (projectile.ai[0] != -1)
				projectile.ai[0] = -1;
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			drawCacheProjsBehindNPCsAndTiles.Add(index);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Player player = Main.player[projectile.owner];

			Texture2D tailPartTex = GetTexture("MysticHunter/Souls/Data/GiantWormSoulProj_Chain");

			Vector2 origin = new Vector2(tailPartTex.Width / 2, tailPartTex.Height / 2);

			float startRot = -projectile.rotation;

			int dir = projectile.rotation == MathHelper.PiOver2 ? 1 : -1;
			Vector2 startPos = projectile.position + new Vector2(dir == 1 ? projectile.width + 6 : 0, projectile.height / 2);
			Vector2 endPos = player.Center + new Vector2(0, 10);

			int maxLen = 1;

			for (int i = 0; i < maxLen; ++i)
			{
				// Draw the current tail part/piece.
				spriteBatch.Draw(tailPartTex, startPos - Main.screenPosition, null, lightColor, startRot + (float)Math.PI, origin, tailScale, SpriteEffects.None, 0);

				// Calculate two rotational options:
				// 1. A remi-preset rotation which is dependant on the index of the current tail piece.
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

				if (i == maxLen - 1 && maxLen < tailParts && Vector2.Distance(startPos, endPos) > (tailPartLength/2) * tailScale)
					maxLen++;
			}

			return (true);
		}
	}
}
