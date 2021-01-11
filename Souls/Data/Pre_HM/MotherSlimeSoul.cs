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
	public class MotherSlimeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.MotherSlime;
		public override string soulDescription => "Summons a massive slime blob.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 50;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<MotherSlimeSoulProj>() && Main.projectile[i].owner == p.whoAmI)
					Main.projectile[i].Kill();

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<MotherSlimeSoulProj>(), 0, 0, p.whoAmI, stack);
			return (true);
		}
	}

	public class MotherSlimeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.MotherSlime;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mother Blob");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 36;
			projectile.height = 24;

			projectile.alpha = 150;
			projectile.scale = 1.25f;
			projectile.minionSlots = 0;

			projectile.friendly = true;
			projectile.ignoreWater = true;

			drawOriginOffsetY = -4;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.MotherSlime)
				projectile.timeLeft = 2;

			// A cooldown of 20 seconds (1200 ticks) - 1 second per soul stack (minimum of 720 ticks/12 seconds).
			float summonCooldown = 1260 - (60 * projectile.ai[0]);

			// Spawn a new mini slime.
			if (Main.myPlayer == projectile.owner && projectile.ai[1]++ >= summonCooldown)
			{
				int damage = (int)(20 + projectile.ai[0] * 2);
				Vector2 velocity = new Vector2(Main.rand.Next(7) - 3, -4);

				Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<MiniSlimeProj>(), damage, .2f, owner.whoAmI);
				projectile.ai[1] = 0;
			}

			// Apply gravity.
			projectile.velocity.Y += .2f;

			// Animate projectile.
			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			return (false);
		}

		public override bool? CanDamage()
			=> false;

		public override Color? GetAlpha(Color lightColor)
			=> new Color(100, 100, 100, projectile.alpha);

		public override bool OnTileCollide(Vector2 oldVelocity) => false;
	}

	public class MiniSlimeProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.BlueSlime;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mini slime");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;

			projectile.alpha = 150;
			projectile.scale = .5f;
			projectile.timeLeft = 600;
			projectile.penetrate = -1;

			projectile.friendly = true;

			drawOriginOffsetY = -2;
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
			=> new Color(100, 100, 100, projectile.alpha);

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
			SoundEngine.PlaySound(SoundID.NPCDeath1, projectile.Center);
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100, new Color(140, 140, 140), projectile.scale);
		}
	}
}
