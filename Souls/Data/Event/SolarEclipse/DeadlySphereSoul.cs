#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class DeadlySphereSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.DeadlySphere;
		public override string soulDescription => "Summon Deadly Spheres to protect you.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 100;
			int amount = 1;

			if (stack >= 5)
			{
				amount++;
				damage += 20;
			}
			if (stack >= 9)
			{
				amount++;
				damage += 25;
			}

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.owner == p.whoAmI && proj.type == ModContent.ProjectileType<DeadlySphereSoul_Proj>())
				{
					proj.Kill();
				}
			}

			for (int i = 0; i < amount; ++i)
			{
				Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<DeadlySphereSoul_Proj>(), damage, 0.2f, p.whoAmI);
			}

			return (true);
		}
	}

	public class DeadlySphereSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.DeadlySphere;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Deadly Sphere");
			
			Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.DeadlySphere];
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.DeadlySphere);

			projectile.minionSlots = 0.5f;

			aiType = ProjectileID.DeadlySphere;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.DeadlySphere)
				projectile.timeLeft = 2;

			float swarmAcceleration = 0.05f;
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				bool isSame = Main.projectile[i].type == projectile.type;
				if (i != projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && isSame &&
					Math.Abs(projectile.position.X - Main.projectile[i].position.X) + Math.Abs(projectile.position.Y - Main.projectile[i].position.Y) < (float)projectile.width)
				{
					if (projectile.position.X < Main.projectile[i].position.X)
					{
						projectile.velocity.X -= swarmAcceleration;
					}
					else
					{
						projectile.velocity.X += swarmAcceleration;
					}
					if (projectile.position.Y < Main.projectile[i].position.Y)
					{
						projectile.velocity.Y -= swarmAcceleration;
					}
					else
					{
						projectile.velocity.Y += swarmAcceleration;
					}
				}
			}

			return (true);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			bool hasCollision = false;

			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = oldVelocity.X * -1;
				hasCollision = true;
			}
			if (projectile.velocity.Y != oldVelocity.Y || projectile.velocity.Y == 0f)
			{
				projectile.velocity.Y = oldVelocity.Y * -0.5f;
				hasCollision = true;
			}

			if (hasCollision)
			{
				float length = oldVelocity.Length() / projectile.velocity.Length();
				if (length == 0f)
				{
					length = 1f;
				}
				projectile.velocity /= length;
				if (projectile.ai[0] == 7f && projectile.velocity.Y < -0.1)
				{
					projectile.velocity.Y += 0.1f;
				}
				if (projectile.ai[0] >= 6f && projectile.ai[0] < 9f)
				{
					Collision.HitTiles(projectile.position, oldVelocity, projectile.width, projectile.height);
				}
			}
			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			this.DrawAroundOrigin(spriteBatch, lightColor);
			return (false);
		}
	}
}
