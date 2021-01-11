#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class BlueCultistArcherSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.CultistArcherBlue;
		public override string soulDescription => "Summons friendly cultist archers.";

		public override short cooldown => 480;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;
			int damage = 70 + 2 * stack;

			Vector2 targetVelocity = Main.MouseWorld;
			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = p.Center;

				// Hardcoded, since I don't have the good patience to figure it out atm.
				if (amount == 2)
					spawnPos += new Vector2(-30 * (i == 0 ? 1 : -1), 0);
				else if (amount == 3)
					spawnPos += new Vector2(-30 * (1 - i), 0);

				Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<BlueCultistArcherSoulNPC>(), damage, 0f, p.whoAmI, targetVelocity.X, targetVelocity.Y);
			}

			return (true);
		}
	}

	public class BlueCultistArcherSoulNPC : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.CultistArcherBlue;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cultist Archer");
			Main.projFrames[projectile.type] = 12;
		}
		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 40;

			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;

			drawOffsetX = -22;
			drawOriginOffsetY = -14;
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
				if (Main.myPlayer == projectile.owner)
				{
					// Spawn the required projectile using the cached `velocity` values in ai[2] and ai[3].
					if (projectile.localAI[0] == 40)
					{
						Vector2 velocity = Vector2.Normalize(new Vector2(projectile.ai[0] - projectile.Center.X, projectile.ai[1] - projectile.Center.Y)) * 8;

						int proj = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.WoodenArrowFriendly, projectile.damage, .1f, projectile.owner);
						Main.projectile[proj].noDropItem = true;
						Main.projectile[proj].netUpdate = true;
					}
				}

				// Kill off the Projectile.
				if (projectile.localAI[0] >= 80)
					projectile.Kill();
			}

			projectile.direction = Math.Sign(projectile.ai[0] - projectile.Center.X);
			projectile.spriteDirection = -projectile.direction;

			if (projectile.velocity.Y != 0)
				projectile.frame = 1;
			else
			{
				if (projectile.localAI[0] >= 30 && projectile.localAI[0] < 50)
				{
					Vector2 rotVec = new Vector2(projectile.ai[0] - projectile.Center.X, projectile.ai[1] - projectile.Center.Y);

					if (Math.Abs(rotVec.Y) > Math.Abs(rotVec.X) * 2)
					{
						if (rotVec.Y > 0)
							projectile.frame = 2;
						else
							projectile.frame = 6;
					}
					else if (Math.Abs(rotVec.X) > Math.Abs(rotVec.Y) * 2)
						projectile.frame = 4;
					else if (rotVec.Y > 0)
						projectile.frame = 3;
					else
						projectile.frame = 5;
				}
				else
					projectile.frame = 0;
			}

			if (projectile.velocity.Y == 0)
				projectile.localAI[0]++;
			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + .2f, -8, 8);

			return (false);
		}

		public override bool? CanDamage()
			=> false;

		public override bool OnTileCollide(Vector2 oldVelocity) => false;
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}

		public override void Kill(int timeLeft)
			=> DustEffect();

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
