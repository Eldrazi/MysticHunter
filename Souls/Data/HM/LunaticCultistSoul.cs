using System;
using System.Linq;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class CultistDevoteSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.CultistDevote;
		public override string soulDescription => "Summons cheerleading cultists.";

		public override short cooldown => 1800;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = p.Center;

				// Hardcoded, since I don't have the good patience to figure it out atm.
				if (amount == 2)
					spawnPos += new Vector2(-40 * (i == 0 ? 1 : -1), 0);
				else if (amount == 3)
					spawnPos += new Vector2(-40 * (1 - i), 0);

				Projectile.NewProjectile(spawnPos, Vector2.Zero, ProjectileType<CultistDevoteSoulProj>(), 0, 0f, p.whoAmI, i, stack);
			}

			return (true);
		}

		public override void PostUpdate(Player player)
		{
			int cultistCount = Main.projectile.Where(x =>
				x.type == ProjectileType<CultistDevoteSoulProj>() &&
				x.owner == player.whoAmI).Count();

			player.statDefense += 5 * cultistCount;
			player.meleeDamage += .33f * cultistCount;
			player.magicDamage += .33f * cultistCount;
			player.rangedDamage += .33f * cultistCount;
			player.minionDamage += .33f * cultistCount;
			player.thrownDamage += .33f * cultistCount;
		}
	}

	public class CultistDevoteSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.CultistDevote;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cultist");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.CultistDevote];
		}
		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 40;

			projectile.scale = .8f;
			projectile.timeLeft = 600;

			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = true;
			projectile.manualDirectionChange = true;

			drawOffsetX = -22;
			drawOriginOffsetY = -14;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				DustEffect();
				projectile.localAI[0] = 1;
			}

			if (projectile.velocity.Y != 0)
				projectile.frame = 2;
			else
			{
				if (projectile.frameCounter++ >= 10)
				{
					projectile.frameCounter = 0;
					projectile.frame += (int)projectile.localAI[0];

					if ((projectile.frame <= 0 && projectile.localAI[0] == -1) ||
						(projectile.frame >= Main.projFrames[projectile.type] - 1 && projectile.localAI[0] == 1))
						projectile.localAI[0] *= -1;

					if (projectile.frame == 3)
					{
						for (int i = 0; i < 5; ++i)
						{
							int randDustType = Utils.SelectRandom<int>(Main.rand, 139, 140, 141, 142);
							Dust.NewDustPerfect(projectile.Center + new Vector2(-6, -10), randDustType, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * -4);
						}
					}
				}
			}

			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + .2f, -8, 8);
			projectile.direction = projectile.spriteDirection = Math.Sign(projectile.Center.X - Main.player[projectile.owner].Center.X);

			return (false);
		}

		public override bool CanDamage() => false;

		public override bool OnTileCollide(Vector2 oldVelocity) => false;
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}

		public override void Kill(int timeLeft) => DustEffect();

		private void DustEffect()
		{
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, .6f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .3f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
