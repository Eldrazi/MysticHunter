using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class LightMummySoul : PostHMSoul
	{
		public override short soulNPC => NPCID.LightMummy;
		public override string soulDescription => "Summons a shambling Light Mummy.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30 + 5 * stack;
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;
			
			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = p.Center + new Vector2(0, 48);

				if (amount == 2)
					spawnPos += new Vector2(-30 * (i == 0 ? 1 : -1), 0);
				else if (amount == 3)
					spawnPos += new Vector2(-30 * (1 - i), 0);

				Projectile proj = Main.projectile[Projectile.NewProjectile(spawnPos, Vector2.Zero, ProjectileType<LightMummySoulProj>(), damage, 0, p.whoAmI)];
				proj.direction = p.direction;
				proj.netUpdate = true;
			}
			return (true);
		}
	}

	public class LightMummySoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_80";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Light Mummy");
			Main.projFrames[projectile.type] = 15;
		}
		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 52;

			projectile.penetrate = -1;
			projectile.timeLeft = 600;

			projectile.hide = true;
			projectile.melee = true;
			projectile.friendly = true;
			projectile.manualDirectionChange = true;
		}

		public override bool PreAI()
		{
			// Set the correct direction of the projectile.
			projectile.spriteDirection = -projectile.direction;

			if (projectile.ai[0] == 0)
			{
				projectile.frame = 1;
				projectile.velocity.Y = -.8f;
				projectile.tileCollide = false;

				bool isBehindTiles = Collision.SolidTiles((int)projectile.position.X / 16, (int)(projectile.position.X + projectile.width) / 16,
					(int)projectile.position.Y / 16, (int)(projectile.position.Y + projectile.height) / 16);

				if (!isBehindTiles)
				{
					projectile.ai[0] = 1;
					projectile.ai[1] = 0;
					projectile.velocity.Y = 0;
				}
				else
					TileEffects();
			}
			else if (projectile.ai[0] == 1)
			{
				projectile.tileCollide = true;

				// Animate the projectile.
				if (projectile.velocity.Y != 0)
				{
					projectile.frame = 0;
					projectile.frameCounter = 0;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					if (projectile.frameCounter++ >= 4)
					{
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
	
						projectile.frameCounter = 0;
					}
					if (projectile.frame == 0)
						projectile.frame = 1;
				}

				projectile.velocity.Y += .2f;
				projectile.velocity.X = projectile.direction;

				// If the projectile is almost dead, make it disappear into the ground.
				if (projectile.timeLeft <= 60)
					projectile.ai[0] = 2;

				ConvertTiles();
			}
			else
			{
				if (projectile.timeLeft > 60)
					projectile.timeLeft = 60;

				// If the projectile is 'falling' kill it off immediately.
				if (projectile.velocity.Y > 1)
					projectile.Kill();

				projectile.frame = 0;

				projectile.velocity.X = 0;
				projectile.velocity.Y = .8f;
				projectile.tileCollide = false;

				TileEffects();
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.ai[0] == 1)
			{
				if (projectile.velocity.X != oldVelocity.X)
					projectile.ai[0] = 2;
			}
			return (false);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			drawCacheProjsBehindNPCsAndTiles.Add(index);
		}

		private void TileEffects()
		{
			// Digging visual effect.
			int minX = (int)MathHelper.Clamp((projectile.position.X / 16f) - 1, 0, Main.maxTilesX);
			int maxX = (int)MathHelper.Clamp((projectile.position.X + projectile.width) / 16f + 2, 0, Main.maxTilesX);
			int minY = (int)MathHelper.Clamp(projectile.position.Y / 16f - 1, 0, Main.maxTilesY);
			int maxY = (int)MathHelper.Clamp((projectile.position.Y + projectile.height) / 16f + 2, 0, Main.maxTilesY);
			for (int x = minX; x < maxX; x++)
			{
				for (int y = minY; y < maxY; y++)
				{
					if (Main.tile[x, y] != null && Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type])
					{
						Vector2 vector = new Vector2(x * 16, y * 16);

						if (projectile.position.X + projectile.width > vector.X && projectile.position.X < vector.X + 16f && projectile.position.Y + projectile.height > vector.Y && projectile.position.Y < vector.Y + 16f)
						{
							if (Main.rand.Next(50) == 0 && Main.tile[x, y].nactive())
								WorldGen.KillTile(x, y, true, true);
						}
					}
				}
			}

			// Digging sound effect.
			projectile.soundDelay = 20;
			Main.PlaySound(15, (int)projectile.position.X, (int)projectile.position.Y);
		}

		private void ConvertTiles()
		{
			int minX = (int)(projectile.position.X / 16) - 2;
			int maxX = (int)((projectile.position.X + projectile.width) / 16) + 2;

			int minY = (int)(projectile.position.Y / 16) - 2;
			int maxY = (int)((projectile.position.Y + projectile.height) / 16) + 2;

			if (minX < 0)
				minX = 0;
			if (maxX > Main.maxTilesX)
				maxX = Main.maxTilesX;

			if (minY < 0)
				minY = 0;
			if (maxY > Main.maxTilesY)
				maxY = Main.maxTilesY;

			for (int x = minX; x <= maxX; ++x)
			{
				for (int y = minY; y <= maxY; ++y)
				{
					if (Main.myPlayer == projectile.owner && Main.tile[x, y] != null)
					{
						if (Main.tile[x, y].type == TileID.CorruptGrass || Main.tile[x, y].type == TileID.FleshGrass || Main.tile[x, y].type == TileID.Grass)
						{
							Main.tile[x, y].type = TileID.HallowedGrass;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						else if (Main.tile[x, y].type == TileID.Ebonsand || Main.tile[x, y].type == TileID.Crimsand || Main.tile[x, y].type == TileID.Sand)
						{
							Main.tile[x, y].type = TileID.Pearlsand;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						else if (Main.tile[x, y].type == TileID.Ebonstone || Main.tile[x, y].type == TileID.Crimstone || Main.tile[x, y].type == TileID.Stone)
						{
							Main.tile[x, y].type = TileID.Pearlstone;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						else if (Main.tile[x, y].type == TileID.CorruptIce || Main.tile[x, y].type == TileID.FleshIce || Main.tile[x, y].type == TileID.IceBlock)
						{
							Main.tile[x, y].type = TileID.HallowedIce;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						else if (Main.tile[x, y].type == TileID.CorruptHardenedSand || Main.tile[x, y].type == TileID.CrimsonHardenedSand || Main.tile[x, y].type == TileID.HardenedSand)
						{
							Main.tile[x, y].type = TileID.HallowHardenedSand;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						else if (Main.tile[x, y].type == TileID.CorruptSandstone || Main.tile[x, y].type == TileID.CrimsonSandstone || Main.tile[x, y].type == TileID.Sandstone)
						{
							Main.tile[x, y].type = TileID.HallowSandstone;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
					}
				}
			}
		}
	}
}
