using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class BeeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Bee;
		public string soulDescription => "Fires a small horde of bees.";

		public short cooldown => 60;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => 5;
		public bool SoulUpdate(Player p, short stack)
		{
			// Spawn 3 to 5 little bee projectiles.
			int min = 3 + stack;
			int max = 6 + (int)(1.5 * stack);
			for (int i = 0; i < Main.rand.Next(min, max); ++i)
			{
				// Get a random position somewhere on the player to spawn a bee.
				Vector2 pos = p.position + new Vector2(Main.rand.Next(0, p.width + 1), Main.rand.Next(0, p.height + 1));

				// Calculate the required velocity of the bees towards the cursor.
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - pos);
				velocity *= 5;

				Projectile.NewProjectile(pos, velocity, ProjectileType<BeeSoulProj>(), 2 + stack, .1f, p.whoAmI);
			}
			return (true);
		}
	}

	public class BeeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_210";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bee");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 4;

			projectile.magic = true;
			projectile.friendly = true;

			if (Main.rand != null)
				projectile.scale = Main.rand.Next(5, 11) * .1f;
		}

		public override bool PreAI()
		{
			// Set the correct direction of the projectile.
			if (projectile.velocity.X > 0)
				projectile.spriteDirection = -1;
			else
				projectile.spriteDirection = 1;

			// Set the correct rotation of the projectile.
			projectile.rotation = projectile.velocity.X * 0.2f;

			// Animate the projectile.
			if (projectile.frameCounter++ >= 3)
			{
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				projectile.frameCounter = 0;
			}
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.Poisoned, 90);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Grass, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
