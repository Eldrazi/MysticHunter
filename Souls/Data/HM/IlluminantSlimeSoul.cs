#region Using directives

using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class IlluminantSlimeSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.IlluminantSlime;
		public override string soulDescription => "Fires a confusing slime blob.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 28;
			int bounceAmount = 1;

			if (stack >= 5)
			{
				damage += 15;
				bounceAmount += 2;
			}
			if (stack >= 9)
			{
				damage += 5;
				bounceAmount += 2;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<IlluminantSlimeSoulProj>(), damage, .2f, p.whoAmI, bounceAmount);

			return (true);
		}
	}

	public class IlluminantSlimeSoulProj : ModProjectile
	{
		Vector2 bounceVelocity;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Confusing Blob");
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

			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -16, 16);

			if (Main.rand.Next(10) == 0)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PinkCrystalShard)];
				d.noGravity = true;
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Confused, 180);
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

			projectile.position.X = projectile.position.X + (projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (projectile.height / 2);
			projectile.width = 32;
			projectile.height = 32;
			projectile.position.X = projectile.position.X - (projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (projectile.height / 2);
			projectile.Damage();

			for (int i = 0; i < 10; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PinkCrystalShard)];
				d.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
				d.noGravity = true;
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(bounceVelocity.X);
			writer.Write(bounceVelocity.Y);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			this.bounceVelocity.X = reader.ReadSingle();
			this.bounceVelocity.Y = reader.ReadSingle();
		}
	}
}
