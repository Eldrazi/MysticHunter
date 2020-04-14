using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class VultureSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Vulture;
		public override string soulDescription => "Summons a swooping vulture.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 8;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			Vector2 maxVelocity = new Vector2(3, 6);

			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = Main.MouseWorld + new Vector2(Main.rand.Next(101) - 50, 0);

				Vector2 velocity = new Vector2(maxVelocity.X * (p.Center.X < spawnPos.X ? 1 : -1), maxVelocity.Y);

				spawnPos -= velocity * Main.rand.Next(60, 90);
				Projectile.NewProjectile(spawnPos, velocity, ProjectileType<VultureSoulProj>(), 5 + stack, .1f, p.whoAmI, Main.MouseWorld.Y);
			}

			// Play 'minion summon' item sound.
			Main.PlaySound(SoundID.Item44, p.position);
			return (true);
		}
	}

	public class VultureSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_61";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vulture");
			Main.projFrames[projectile.type] = 3;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = false;

			projectile.scale = .8f;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1] == 0) // Swoop down state.
			{
				// Check to see if the projectile is almost at its downwards top/deadpoint.
				if (projectile.ai[0] - projectile.Center.Y <= 64)
					projectile.ai[1] = 1;
			}
			else // Swoop up state.
			{
				if (projectile.timeLeft > 60)
					projectile.timeLeft = 60;

				projectile.velocity.Y -= .3f;
				if (projectile.velocity.Y > 0)
					projectile.velocity.Y *= .98f;
			}

			if (projectile.frameCounter++ >= 5)
			{
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				projectile.frameCounter = 0;
			}
			if (projectile.frame == 0)
				projectile.frame = 1;

			projectile.spriteDirection = -projectile.direction;
			if (projectile.velocity.X > 0)
				projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
			else
				projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + (float)Math.PI;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Sandnado, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
