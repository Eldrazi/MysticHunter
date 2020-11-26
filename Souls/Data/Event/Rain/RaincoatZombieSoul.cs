using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.Rain
{
	public class RaincoatZombieSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.ZombieRaincoat;
		public override string soulDescription => "Summons a shambling Zombie that leaves a cursed water trail.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 10 + 2 * stack;
			float modifier = 90;
			if (stack >= 5)
				modifier -= 30;
			if (stack >= 9)
				modifier -= 30;
			
			Vector2 spawnPos = p.Center + new Vector2(0, 48);

			Projectile proj = Main.projectile[Projectile.NewProjectile(spawnPos, Vector2.Zero, ProjectileType<RaincoatZombieSoulProj>(), damage, 0, p.whoAmI, 0, modifier)];
			proj.direction = p.direction;
			proj.netUpdate = true;

			return (true);
		}
	}

	internal sealed class RaincoatZombieSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.ZombieRaincoat;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Raincoat Zombie");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 46;

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
					projectile.frame = 2;
					projectile.frameCounter = 0;
				}
				else
				{
					if (projectile.frameCounter++ >= 10)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];	
					}
				}

				projectile.velocity.Y += .2f;
				projectile.velocity.X = projectile.direction;

				// If the projectile is almost dead, make it disappear into the ground.
				if (projectile.timeLeft <= 60)
					projectile.ai[0] = 2;

				SpawnTrail();
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

		private void SpawnTrail()
		{
			if (projectile.owner == Main.myPlayer && projectile.localAI[0]++ >= projectile.ai[1])
			{
				projectile.localAI[0] = 0;

				Vector2 newProjPos = new Vector2(projectile.Center.X - (16 * projectile.direction), projectile.position.Y + projectile.height);
				Projectile.NewProjectile(newProjPos, Vector2.Zero, ProjectileType<RaincoatZombieSoulProjTrail>(), projectile.damage, 0, projectile.owner);
			}
		}
	}

	internal sealed class RaincoatZombieSoulProjTrail : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.None;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Water Trail");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = 2;
			projectile.timeLeft = 120;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, 0, 0, 100)].noGravity = true;

			Lighting.AddLight(projectile.Center, 0, .3f, 1);
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, 0, 0, 100);
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, 0, 0, 100, default, .5f);
			}
		}
	}
}
