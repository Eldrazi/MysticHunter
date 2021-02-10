#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class AlienHornetSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.VortexHornet;
		public override string soulDescription => "Summon an angry Alien Hornet.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 35;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 170;
			int amount = 1;

			if (stack >= 5)
			{
				damage += 15;
				amount++;
			}
			if (stack >= 9)
			{
				damage += 15;
				amount++;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 10f;
			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(p.Center, velocity.RotatedByRandom(MathHelper.PiOver4 / 2), ModContent.ProjectileType<AlienHornetSoul_Proj>(), damage, 0.5f, p.whoAmI);
			}

			return (true);
		}
	}

	public class AlienHornetSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.VortexHornet;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alien Hornet");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.VortexHornet];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.penetrate = 3;
			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			int target = -1;
			float currentDistance = 600;

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				Vector2 directionTowards = Main.npc[i].Center - projectile.Center;
				if (!Main.npc[i].CanBeChasedBy(projectile) || directionTowards.Length() > currentDistance)
				{
					continue;
				}

				target = i;
				currentDistance = directionTowards.Length();
			}

			if (target != -1)
			{
				Vector2 directionTowards = Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 10;

				if (projectile.velocity.X < directionTowards.X)
				{
					if (projectile.velocity.X < 0)
					{
						projectile.velocity.X *= 0.98f;
					}
					projectile.velocity.X += 0.2f;
				}
				else if (projectile.velocity.X > directionTowards.X)
				{
					if (projectile.velocity.X > 0)
					{
						projectile.velocity.X *= 0.98f;
					}
					projectile.velocity.X -= 0.2f;
				}

				if (projectile.velocity.Y < directionTowards.Y)
				{
					if (projectile.velocity.Y < 0)
					{
						projectile.velocity.Y *= 0.98f;
					}
					projectile.velocity.Y += 0.2f;
				}
				else if (projectile.velocity.Y > directionTowards.Y)
				{
					if (projectile.velocity.Y < 0)
					{
						projectile.velocity.Y *= 0.98f;
					}
					projectile.velocity.Y -= 0.2f;
				}
			}

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (!Main.projectile[i].active || Main.projectile[i].owner != projectile.owner || Main.projectile[i].type != projectile.type)
				{
					continue;
				}

				Vector2 directionToOther = Main.projectile[i].Center - projectile.Center;

				if (!directionToOther.HasNaNs() && directionToOther != Vector2.Zero && directionToOther.Length() <= 30)
				{
					projectile.velocity -= Vector2.Normalize(directionToOther) * 0.1f;
				}
			}

			projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -10, 10);
			projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -10, 10);

			// Animation.
			if (++projectile.frameCounter >= 3)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 9;
			}
			if (projectile.frame < 6)
			{
				projectile.frame = 6;
			}

			projectile.rotation = projectile.velocity.X * 0.1f;
			projectile.spriteDirection = -projectile.direction;

			if (Main.rand.Next(10) == 0)
			{
				Dust newDust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Vortex, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f);
				newDust.noGravity = true;
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Vortex, 0, 0, 100, default, 2f)];
				d.noGravity = true;
				d.velocity *= 2;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
