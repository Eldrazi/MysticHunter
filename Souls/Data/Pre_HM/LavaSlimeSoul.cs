﻿#region Using directives

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
	public class LavaSlimeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.LavaSlime;
		public override string soulDescription => "Fires a burning, bouncing slime blob.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 12;
			int bounceAmount = 1;

			if (stack >= 5)
			{
				damage += 5;
				bounceAmount += 2;
			}
			if (stack >= 9)
			{
				damage += 5;
				bounceAmount += 2;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<LavaSlimeSoulProj>(), damage, .2f, p.whoAmI, bounceAmount);

			return (true);
		}
	}

	public class LavaSlimeSoulProj : ModProjectile
	{
		Vector2 bounceVelocity;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burning Blob");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1] == 0)
			{
				projectile.frame = 0;
				projectile.velocity.Y += .06f;
				projectile.rotation += projectile.velocity.Length() * .1f * projectile.direction;

				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100, default, 2f)];
				d.noGravity = true;
				d.velocity *= .3f;
			}
			else
			{
				if (projectile.localAI[0]++ >= 10)
				{
					projectile.ai[1] = 0;
					projectile.localAI[0] = 0;

					// Set the projectiles' velocity to the calculated bounceVelocity.
					projectile.velocity = bounceVelocity;
				}
				projectile.frame = 1;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(10) == 0)
				target.AddBuff(BuffID.OnFire, 180);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.ai[0] > 0)
			{
				SoundEngine.PlaySound(SoundID.NPCHit1, projectile.position);
				bounceVelocity = oldVelocity;
				projectile.position += oldVelocity;
				if (oldVelocity.X != projectile.velocity.X)
				{
					bounceVelocity.X *= -1;
					if (oldVelocity.X >= 0)
						projectile.rotation = -MathHelper.PiOver2;
					else
						projectile.rotation = MathHelper.PiOver2;
				}
				if (oldVelocity.Y != projectile.velocity.Y)
				{
					bounceVelocity.Y *= -1;
					if (oldVelocity.Y >= 0)
						projectile.rotation = 0;
					else
						projectile.rotation = (float)Math.PI;
				}

				projectile.ai[0]--;
				projectile.ai[1] = 1;
				projectile.velocity *= 0;
				projectile.netUpdate = true;
				return (false);
			}
			return (true);
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath1, projectile.position);
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 180, Color.OrangeRed);
		}
	}
}
