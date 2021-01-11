#region Using directives

using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.BloodMoon
{
	public class BloodZombieSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.BloodZombie;
		public override string soulDescription => "Summons a lumbering Blood Zombie.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(5 + 3 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 20 + 3 * stack;
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

				Projectile proj = Main.projectile[Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<BloodZombieSoulProj>(), damage, 0, p.whoAmI, 0, stack)];
				proj.direction = p.direction;
				proj.netUpdate = true;
			}
			return (true);
		}
	}

	public class BloodZombieSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.BloodZombie;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Zombie");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.BloodZombie];
		}
		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 42;

			projectile.penetrate = -1;
			projectile.timeLeft = 600;

			projectile.hide = true;
			projectile.friendly = true;
			projectile.manualDirectionChange = true;
		}

		public override bool PreAI()
		{
			// Set the correct direction of the projectile.
			projectile.spriteDirection = -projectile.direction;

			if (projectile.ai[0] == 0)
			{
				projectile.tileCollide = false;
				projectile.velocity.Y = -.8f;

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
					projectile.frame = 0;
					projectile.frameCounter = 0;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					if (projectile.frameCounter++ >= 12)
					{
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
						projectile.frameCounter = 0;
					}

					if (projectile.frame == 0)
					{
						projectile.frame++;
					}
				}

				projectile.velocity.Y += .2f;
				projectile.velocity.X = projectile.direction;

				// If the projectile is almost dead, make it disappear into the ground.
				if (projectile.timeLeft <= 60)
					projectile.ai[0] = 2;
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
			=> drawCacheProjsBehindNPCsAndTiles.Add(index);

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Type].Value;
			Rectangle frame = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
			Vector2 origin = frame.Size();
			SpriteEffects effects = projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(tex, projectile.Center + origin/2 - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effects, 0f);

			return (false);
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
			SoundEngine.PlaySound(15, (int)projectile.position.X, (int)projectile.position.Y);
		}
	}
}
