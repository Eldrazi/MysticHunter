using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class HornetSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Hornet;
		public override string soulDescription => "Summons a stinger shooting Hornet.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<HornetSoulProj>())
					Main.projectile[i].Kill();

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<HornetSoulProj>(), 10 + 2 * stack, 0, p.whoAmI, stack);
			return (true);
		}
	}

	public class HornetSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Hornet;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hive");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0f;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			int stack = (int)projectile.ai[0];
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoulNet.soulNPC == NPCID.Hornet)
				projectile.timeLeft = 2;

			// Position the projectile correctly (above the player).
			projectile.position.X = owner.Center.X - (projectile.width * .5f);
			projectile.position.Y = owner.Center.Y - (projectile.height * .5f) + owner.gfxOffY - 50f;

			if (owner.gravDir == -1)
			{
				projectile.position.Y += 100;
				projectile.rotation = (float)Math.PI;
			}
			else
				projectile.rotation = 0;

			projectile.rotation += (owner.velocity.X * .1f) * owner.gravDir;

			projectile.position.X = (int)projectile.position.X;
			projectile.position.Y = (int)projectile.position.Y;

			// Scaling the projectile.
			float scale = (Main.mouseTextColor / 200 - .35f) * .1f;
			projectile.scale = scale + .8f;

			// Animate the projectile.
			projectile.spriteDirection = -owner.direction;
			if (projectile.frameCounter++ >= 6)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			// Child/sub projectile spawning.
			int cooldown = 600 - (stack * 60);
			if (owner.whoAmI == Main.myPlayer && projectile.ai[1]++ >= cooldown)
			{
				bool shot = false;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC target = Main.npc[i];
					if (target.CanBeChasedBy() && Collision.CanHit(projectile.position, projectile.width, projectile.height,
						target.position, target.width, target.height))
					{
						Vector2 velocity = Vector2.Normalize(target.Center - projectile.Center) * 8;

						Projectile newProj = Main.projectile[Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.Stinger, projectile.damage, .1f, owner.whoAmI)];
						newProj.timeLeft = 300;
						newProj.friendly = true;
						newProj.netUpdate = true;

						shot = true;
					}
				}

				if (shot)
				{
					projectile.ai[1] = 0;
					projectile.netUpdate = true;
				}
			}

			return (false);
		}

		public override bool CanDamage() => false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Honey, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
