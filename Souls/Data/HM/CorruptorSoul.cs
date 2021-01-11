#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class CorruptorSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Corruptor;
		public override string soulDescription => "Spit out corrupting spit.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30 + (5 * stack);

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<CorruptorSoulProj>(), damage, .2f, p.whoAmI, stack);

			return (true);
		}
	}

	public class CorruptorSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.VileSpit;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrupt Spit");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = true;

			projectile.scale = .9f;
		}

		public override bool PreAI()
		{
			projectile.ai[0]++;
			if (projectile.ai[0] > 3f)
				projectile.ai[0] = 3f;
			if (projectile.ai[0] == 2f)
			{
				projectile.position += projectile.velocity;
				SoundEngine.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 9);

				for (int i = 0; i < 20; ++i)
				{
					Dust d = Main.dust[Dust.NewDust(projectile.position + new Vector2(0, 2), projectile.width, projectile.height, 18, 0f, 0f, 100, default, 1.8f)];
					d.velocity = (d.velocity * 1.3f) + projectile.velocity;
					d.noGravity = true;
				}
			}

			for (int i = 0; i < 2; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position + new Vector2(0, 2), projectile.width, projectile.height, 18, projectile.velocity.X * .1f, projectile.velocity.Y * .1f, 80, default, 1.3f)];
				d.velocity *= .3f;
				d.noGravity = true;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath9, projectile.position);
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position + new Vector2(0, 2), projectile.width, projectile.height, 18, 0f, 0f, 100, default, 1.8f)];
				d.velocity = (d.velocity * 1.3f) + projectile.velocity;
				d.noGravity = true;
			}

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
						if (Main.tile[x, y].type == TileID.HallowedGrass || Main.tile[x, y].type == TileID.Grass)
						{
							Main.tile[x, y].type = TileID.CorruptGrass;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						if (Main.tile[x, y].type == TileID.Pearlsand || Main.tile[x, y].type == TileID.Sand)
						{
							Main.tile[x, y].type = TileID.Ebonsand;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						if (Main.tile[x, y].type == TileID.Pearlstone || Main.tile[x, y].type == TileID.Stone)
						{
							Main.tile[x, y].type = TileID.Ebonstone;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						if (Main.tile[x, y].type == TileID.HallowedIce || Main.tile[x, y].type == TileID.IceBlock)
						{
							Main.tile[x, y].type = TileID.CorruptIce;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						if (Main.tile[x, y].type == TileID.HallowHardenedSand || Main.tile[x, y].type == TileID.HardenedSand)
						{
							Main.tile[x, y].type = TileID.CorruptHardenedSand;
							WorldGen.SquareTileFrame(x, y);
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendTileSquare(-1, x, y, 1);
						}
						if (Main.tile[x, y].type == TileID.HallowSandstone || Main.tile[x, y].type == TileID.Sandstone)
						{
							Main.tile[x, y].type = TileID.CorruptSandstone;
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
