using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class SlimelingSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Slimeling;
		public override string soulDescription => "Summons a Slimeling friend.";

		public override short cooldown => 30;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60;
			float knockBack = .25f;

			if (stack >= 5)
				damage += 10;
			if (stack >= 9)
				damage += 10;

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<SlimelingSoulProj>(), damage, knockBack, p.whoAmI);
			return (true);
		}
	}

	public class SlimelingSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_1";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slimeling");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;

			projectile.penetrate = -1;

			projectile.minion = true;
			projectile.friendly = true;

			projectile.scale = .9f;
			projectile.timeLeft = 600;
		}

		public override bool PreAI()
		{
			if (projectile.direction == 0)
				projectile.direction = 1;

			if (projectile.velocity.Y == 0)
			{
				projectile.velocity.X *= .9f;
				if (projectile.velocity.X > -.1f && projectile.velocity.X < .1f)
					projectile.velocity.X = 0;

				if (projectile.ai[0]++ >= 120 && projectile.owner == Main.myPlayer)
				{
					// 50/50 change to go left or right.
					if (Main.rand.Next(2) == 0)
						projectile.direction *= -1;

					projectile.velocity.Y = -5f;
					projectile.velocity.X = 3 * projectile.direction;

					projectile.ai[0] = 0 - Main.rand.Next(0, 91);
					projectile.netUpdate = true;
				}
			}

			// Animate.
			if (projectile.ai[0] >= 60 || projectile.velocity.Y != 0)
				projectile.frameCounter += 4;

			if (projectile.frameCounter++ >= 12)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			// Apply gravity.
			projectile.velocity.Y += .2f;
			return (false);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return (new Color(0, 58, 52, 134));
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
				projectile.velocity.X *= -1;

			if (projectile.velocity.Y != oldVelocity.Y && projectile.velocity.Y == 0)
				projectile.position.X -= projectile.velocity.X;
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GreenBlood, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);

			Main.PlaySound(SoundID.NPCDeath1, projectile.Center);
		}
	}
}
