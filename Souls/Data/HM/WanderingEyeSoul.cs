using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class WanderingEyeSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.WanderingEye;
		public override string soulDescription => "Fire a bouncing Wandering Eye.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30 + 5 * stack;
			int bounce = 3 + (stack / 2);

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 5f;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<WanderingEyeSoulProj>(), damage, 1, p.whoAmI, bounce);

			return (true);
		}
	}

	public class WanderingEyeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.WanderingEye;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wandering Eye");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.WanderingEye];
		}
		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 36;

			projectile.scale = .8f;
			projectile.timeLeft = 10;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
		}

		public override bool PreAI()
		{
			projectile.rotation = projectile.velocity.ToRotation() + (float)System.Math.PI;

			if (projectile.ai[1]++ >= 90)
			{
				if (projectile.velocity.Length() < 12)
					projectile.velocity *= 1.02f;

				if (projectile.frameCounter++ >= 10)
					projectile.frame++;
				if (projectile.frame < 2 || projectile.frame > 3)
					projectile.frame = 2;
			}
			else
			{
				if (projectile.frameCounter++ >= 10)
					projectile.frame = (projectile.frame + 1) % 2;
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.ai[0]--;
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.ai[0]--;
				projectile.velocity.Y = -oldVelocity.Y;
			}

			if (projectile.ai[0] <= 0)
				projectile.Kill();

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.NPCDeath1, projectile.position);
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
