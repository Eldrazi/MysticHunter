using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class HopliteSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.GreekSkeleton;
		public override string soulDescription => "Summons a hoplite.";

		public override short cooldown => 480;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;
			int damage = 20 + (5 * amount);

			Vector2 targetVelocity = Main.MouseWorld;
			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = p.Center;

				// Hardcoded, since I don't have the good patience to figure it out atm.
				if (amount == 2)
					spawnPos += new Vector2(-30 * (i == 0 ? 1 : -1), 0);
				else if (amount == 3)
					spawnPos += new Vector2(-30 * (1 - i), 0);

				Projectile.NewProjectile(spawnPos, Vector2.UnitY, ProjectileType<HopliteSoulNPC>(), damage, 0, p.whoAmI, targetVelocity.X, targetVelocity.Y);
			}

			return (true);
		}
	}

	public class HopliteSoulNPC : ModProjectile
	{
		public override string Texture => "Terraria/NPC_481";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hoplite");
			Main.projFrames[projectile.type] = 19;
		}
		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 40;

			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;

			drawOffsetX = -12;
			drawOriginOffsetY = -24;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[1] == 0)
			{
				DustEffect();
				projectile.localAI[1] = 1;
			}
			else if (projectile.localAI[0] >= 30)
			{
				if (projectile.owner == Main.myPlayer)
				{
					// Spawn the required projectile using the cached `velocity` values in ai[0] and ai[1].
					if (projectile.localAI[0] == 40)
					{
						Vector2 velocity = Vector2.Normalize(new Vector2(projectile.ai[0] - projectile.Center.X, projectile.ai[1] - projectile.Center.Y)) * 8;

						Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.JavelinFriendly, projectile.damage, .1f, projectile.owner);
					}

					// Kill off the Projectile.
					if (projectile.localAI[0] >= 80)
						projectile.Kill();
				}
			}

			projectile.direction = Math.Sign(projectile.ai[0] - projectile.Center.X);
			projectile.spriteDirection = -projectile.direction;

			if (projectile.velocity.Y != 0)
				projectile.frame = 0;
			else
			{
				if (projectile.localAI[0] >= 30 && projectile.localAI[0] < 50)
				{
					int frame = (int)Math.Floor((projectile.localAI[0] - 30) / 5);
					projectile.frame = (15 + frame);
				}
				else
					projectile.frame = 1;
			}

			if (projectile.velocity.Y == 0)
				projectile.localAI[0]++;
			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + .2f, -8, 8);

			return (false);
		}

		public override bool CanDamage() => false;

		public override bool OnTileCollide(Vector2 oldVelocity) => false;
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}

		public override void Kill(int timeLeft)
		{
			DustEffect();
		}

		private void DustEffect()
		{
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, .8f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
