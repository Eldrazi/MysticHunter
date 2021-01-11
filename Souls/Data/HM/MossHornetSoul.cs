#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class MossHornetSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.MossHornet;
		public override string soulDescription => "Summons a swooping Moss Hornet.";

		public override short cooldown => 420;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60;
			Vector2 maxVelocity = new Vector2(3, 8);

			Vector2 spawnPos = Main.MouseWorld + new Vector2(Main.rand.Next(101) - 50, 0);

			Vector2 velocity = new Vector2(maxVelocity.X * (p.Center.X < spawnPos.X ? 1 : -1), maxVelocity.Y);

			spawnPos -= velocity * Main.rand.Next(60, 90);
			Projectile.NewProjectile(spawnPos, velocity, ModContent.ProjectileType<MossHornetSoulProj>(), damage, .1f, p.whoAmI, Main.MouseWorld.Y);

			// Play 'minion summon' item sound.
			SoundEngine.PlaySound(SoundID.Item44, p.position);
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.TinyMossHornet, NPCID.LittleMossHornet, NPCID.GiantMossHornet, NPCID.BigMossHornet };
	}

	public class MossHornetSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.MossHornet;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moss Hornet");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.MossHornet];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1] == 0) // Swoop down state.
			{
				// Check to see if the projectile is almost at its downwards top/deadpoint.
				if (projectile.ai[0] - projectile.Center.Y <= 96)
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

			// Animation
			if (projectile.frameCounter++ >= 5)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			projectile.spriteDirection = -projectile.direction;
			if (projectile.velocity.X > 0)
				projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
			else
				projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + (float)Math.PI;

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(4) == 0)
				target.AddBuff(BuffID.Venom, 240);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.GrassBlades, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
