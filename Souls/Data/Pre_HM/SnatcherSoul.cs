using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SnatcherSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Snatcher;
		public override string soulDescription => "Summons a snatching vine.";

		public override short cooldown => 240;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(10 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 6 + (stack / 3) * 2;
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 28;

			Projectile.NewProjectile(p.Center + velocity, velocity, ProjectileType<SnatcherSoulThornProj>(), 15 + stack, .1f, p.whoAmI, 0, amount);
			return (true);
		}
	}

	public class SnatcherSoulThornProj : ModProjectile
	{
		public override string Texture => "Terraria/Chain5";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorns");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 28;

			projectile.aiStyle = 4;
			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (projectile.ai[0] == 0)
			{
				projectile.alpha -= 50;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.ai[0] = 1;

					if (Main.myPlayer == projectile.owner)
					{
						int type = projectile.type;
						Vector2 spawnPos = projectile.position + projectile.velocity;

						if (projectile.ai[1] <= 1)
						{
							type = ProjectileType<SnatcherSoulHeadProj>();
							spawnPos += new Vector2(0, projectile.height * .5f);
						}
						else
							spawnPos += new Vector2(projectile.width * .5f, projectile.height * .5f);


						int projIndex = Projectile.NewProjectile(spawnPos,
							projectile.velocity, type, projectile.damage, projectile.knockBack, projectile.owner, 0, projectile.ai[1] - 1);
						NetMessage.SendData(27, -1, -1, null, projIndex);
					}
				}
			}
			else
			{
				if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
				{
					for (int i = 0; i < 3; ++i)
						Dust.NewDust(projectile.position, projectile.width, projectile.height, 18, projectile.velocity.X * .025f, projectile.velocity.Y * .025f, 170, default, 1.2f);
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 170, default, 1.1f);
				}
				projectile.alpha += 5;
				if (projectile.alpha >= 255)
					projectile.Kill();
			}

			return (false);
		}
	}

	public class SnatcherSoulHeadProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_56";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snatcher");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 28;

			projectile.aiStyle = 4;
			projectile.alpha = 255;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + (float)System.Math.PI;

			if (projectile.ai[0] == 0)
			{
				projectile.alpha -= 50;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.ai[0] = 1;
					projectile.ai[1] = 1;
				}
			}
			else
			{
				if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
				{
					for (int i = 0; i < 3; ++i)
						Dust.NewDust(projectile.position, projectile.width, projectile.height, 18, projectile.velocity.X * .025f, projectile.velocity.Y * .025f, 170, default, 1.2f);
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 170, default, 1.1f);
				}
				projectile.alpha += 5;
				if (projectile.alpha >= 255)
					projectile.Kill();

				if (projectile.frameCounter++ >= 5)
				{
					projectile.frameCounter = 0;
					projectile.frame += (int)projectile.ai[1];

					if (projectile.frame == 0 || projectile.frame == Main.projFrames[projectile.type] - 1)
						projectile.ai[1] *= -1;
				}
			}

			return (false);
		}
	}
}
