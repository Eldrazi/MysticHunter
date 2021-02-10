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
	public class AlienQueenSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.VortexHornetQueen;
		public override string soulDescription => "Summon an Alien Queen sentry.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Collision.SolidCollision(Main.MouseWorld, 32, 32))
			{
				return (false);
			}

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<AlienQueenSoul_Proj>())
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			int damage = 190 + 10 * stack;
			Projectile.NewProjectile(Main.MouseWorld, default, ModContent.ProjectileType<AlienQueenSoul_Proj>(), damage, 0, p.whoAmI, stack);

			return (true);
		}
	}

	public class AlienQueenSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.VortexHornetQueen;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alien Queen");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.VortexHornetQueen];
		}
		public override void SetDefaults()
		{
			projectile.width = 36;
			projectile.height = 54;

			projectile.timeLeft = Projectile.SentryLifeTime;

			projectile.sentry = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.VortexHornetQueen)
			{
				projectile.timeLeft = 2;
			}

			// Spawn.
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				Main.PlaySound(SoundID.Item46, projectile.position);

				for (int i = 0; i < 80; ++i)
				{
					Dust newDust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, DustID.Vortex);
					newDust.velocity *= 2f;
					newDust.noGravity = true;
					newDust.scale *= 1.15f;
				}
			}

			projectile.velocity.X = 0f;
			projectile.velocity.Y += 0.2f;
			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}

			int spawnModifier = (int)(10 - projectile.ai[0]) * 40;
			if (++projectile.ai[1] >= spawnModifier)
			{
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];

						if (!npc.active || !npc.CanBeChasedBy(projectile) || !Collision.CanHitLine(projectile.Center, 1, 1, npc.Center, 1, 1) ||
							projectile.Distance(npc.Center) > 600)
						{
							continue;
						}

						Projectile.NewProjectile(projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 4, ModContent.ProjectileType<AlienHornetSoul_Proj>(), projectile.damage, 0.2f, projectile.owner);

						projectile.ai[1] = 0; 
						break;
					}					
				}
			}

			return (false);
		}

		public override bool CanDamage()
			=> false;

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
			=> false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Vortex, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100, default, 2f);
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
