#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class GreenSlimeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.GreenSlime;
		public override string soulDescription => "Summons a little slimy friend.";

		public override short cooldown => 30;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 5;
			float knockBack = .1f;

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<GreenSlimeSoulProj>(), damage, knockBack, p.whoAmI);
			return (true);
		}
	}

	public class GreenSlimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.BlueSlime;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slime");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;

			projectile.penetrate = -1;

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
			return (new Color(0, 220, 40, 100));
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

			SoundEngine.PlaySound(SoundID.NPCDeath1, projectile.Center);
		}
	}
}
