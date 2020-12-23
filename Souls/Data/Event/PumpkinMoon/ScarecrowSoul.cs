#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.PumpkinMoon
{
	public class ScarecrowSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Scarecrow1;
		public override string soulDescription => "Summon a helpful Scarecrow.";

		public override short cooldown => 1200;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60 + 5 * stack;

			int projSpawnType = ModContent.ProjectileType<ScarecrowSoul_ScarecrowProj>();
			int newProj = Projectile.NewProjectile(p.Center, Vector2.Normalize(Main.MouseWorld - p.Center) * 6, ModContent.ProjectileType<ScarecrowSoul_Proj>(), damage, .5f, p.whoAmI, projSpawnType);
			Main.projectile[newProj].timeLeft = 600 + 60 * stack;
			Main.projectile[newProj].netUpdate = true;
			return (true);
		}

		public override short[] GetAdditionalTypes() =>
			new short[] { NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5,
				NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10 };
	}

	internal sealed class ScarecrowSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.Seed;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seed");
		}
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;

			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}

		public override bool PreAI()
		{
			projectile.velocity.X *= .99f;
			projectile.velocity.Y += 0.2f;
			return (false);
		}

		public override bool? CanCutTiles() => false;
		public override bool? CanHitNPC(NPC target) => false;
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X * .8f;
			}

			if (projectile.velocity.Y != oldVelocity.Y)
			{
				if (oldVelocity.Y > 0 && Main.myPlayer == projectile.owner)
				{
					int spawnType = (int)projectile.ai[0];
					Vector2 spawnPos = projectile.position + projectile.oldVelocity;
					spawnPos.X = (int)(spawnPos.X / 16) * 16;
					spawnPos.Y = (int)(spawnPos.Y / 16) * 16;

					if (spawnType == ModContent.ProjectileType<ScarecrowSoul_ScarecrowProj>())
					{
						spawnPos.Y -= 6;
					}

					Projectile.NewProjectile(spawnPos, Vector2.Zero, spawnType, projectile.damage, projectile.knockBack, projectile.owner);
				}
				else if (oldVelocity.Y < 0)
				{
					projectile.velocity.Y = oldVelocity.Y * .8f;
				}
			}
			return (true);
		}
	}

	internal sealed class ScarecrowSoul_ScarecrowProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Scarecrow1;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scarecrow");
			Main.projFrames[projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 48;

			projectile.friendly = true;

			drawOriginOffsetX = -6;
			drawOriginOffsetY = -20;
		}

		public override bool PreAI()
		{
			// Logic
			if (projectile.ai[1]++ >= 90)
			{
				if (Main.myPlayer == projectile.owner)
				{
					int projType = ModContent.ProjectileType<ScarecrowSoul_Proj>();
					int projTypeSpawn = ModContent.ProjectileType<ScarecrowSoul_LifeFruitProj>();

					Vector2 randVelocity = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, -7));

					Projectile.NewProjectile(projectile.Center, randVelocity, projType, projectile.damage, projectile.knockBack, projectile.owner, projTypeSpawn);
				}
				projectile.ai[1] = 0;
			}

			// Visuals
			if (projectile.ai[0] == 0)
			{
				projectile.ai[0] = 1;
				for (int i = 0; i < 15; ++i)
				{
					Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100)];
					if (Main.rand.Next(2) == 0)
					{
						d.scale = .5f;
						d.fadeIn = 1 + Main.rand.Next(10) * .1f;
					}
				}
			}

			projectile.localAI[0] += 0.05f;
			projectile.rotation = (float)Math.Sin(projectile.localAI[0]) * 0.1f;
			return (false);
		}

		public override bool? CanCutTiles() => false;
		public override bool? CanHitNPC(NPC target) => false;
		public override bool CanHitPvp(Player target) => false;
		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle frame = texture.Frame(1, 6, 0, projectile.frame);
			Vector2 origin = frame.Size() / 2 + new Vector2(0, frame.Height / 2);

			spriteBatch.Draw(texture, projectile.position + origin - Main.screenPosition + new Vector2(drawOriginOffsetX, drawOriginOffsetY),
				frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}

	internal sealed class ScarecrowSoul_LifeFruitProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Fruit");
			Main.projFrames[projectile.type] = 9;
		}
		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 40;

			projectile.timeLeft = 600;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0]++ < 60)
			{
				if (++projectile.frameCounter >= 15)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % 2;
				}
			}
			else
			{
				if (projectile.frame < Main.projFrames[projectile.type] - 1)
				{
					if (projectile.frameCounter++ >= 7)
					{
						projectile.frameCounter = 0;
						projectile.frame++;
					}
				}
				else
				{
					Player owner = Main.player[projectile.owner];
					if (Main.myPlayer == owner.whoAmI && owner.Hitbox.Intersects(projectile.Hitbox))
					{
						int healAmount = (owner.statLife + 10 < owner.statLifeMax2) ? 10 : owner.statLifeMax2 - owner.statLife;
						owner.HealEffect(healAmount);
						owner.statLife += healAmount;
						projectile.Kill();
					}
				}
			}
			return (false);
		}

		public override bool? CanCutTiles() => false;
		public override bool? CanHitNPC(NPC target) => false;
		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Grass, 0f, 0f, 100)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
