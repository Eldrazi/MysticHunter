#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System;

#endregion

namespace MysticHunter.Souls.Data.Event.BloodMoon
{
	internal sealed class DripplerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Drippler;
		public override string soulDescription => "Summons a Drippler to drip blood.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 25 + 3 * stack;

			if (Collision.SolidCollision(Main.MouseWorld, 22, 22))
			{
				return (false);
			}

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.owner == p.whoAmI && proj.type == ModContent.ProjectileType<DripplerSoul_Proj>())
				{
					proj.Kill();
					break;
				}
			}

			Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<DripplerSoul_Proj>(), damage, 0f, p.whoAmI);

			return (true);
		}
	}

	internal sealed class DripplerSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Drippler;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Drippler");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.Drippler];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.alpha = 255;
			projectile.timeLeft = 600;
			
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			projectile.alpha -= 10;
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}

			if (Main.myPlayer == projectile.owner && projectile.ai[0]++ >= 60)
			{
				int xOffset = Main.rand.Next(-projectile.width / 2, projectile.width / 2 + 1);
				Projectile newProj = Main.projectile[Projectile.NewProjectile(projectile.Center + new Vector2(xOffset, 0), Vector2.UnitY * 5f, ProjectileID.BloodRain, projectile.damage, 0.5f, projectile.owner)];
				newProj.friendly = true;
				newProj.hostile = false;
				newProj.netUpdate = true;

				projectile.ai[0] = 0;
			}

			projectile.localAI[0] += 0.02f;
			projectile.velocity.Y = (float)Math.Sin(projectile.localAI[0]) * 0.2f;


			if (++projectile.frameCounter >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override bool CanDamage()
			=> false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; ++i)
			{
				Dust newDust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 1.5f)];
				newDust.velocity *= 1.4f;
			}
		}
	}
}
