using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SharkSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Shark;
		public override string soulDescription => "Summons a friendly shark";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<SharkSoulProj>())
					Main.projectile[i].Kill();

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<SharkSoulProj>(), 2 + stack, .1f, p.whoAmI);
			return (true);
		}
	}

	public class SharkSoulProj : ModProjectile
	{
		private float randomRot;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shark");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 70;
			projectile.height = 28;

			projectile.scale = .8f;
			projectile.timeLeft *= 5;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
			projectile.manualDirectionChange = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoulNet.soulNPC == NPCID.Shark)
				projectile.timeLeft = 2;

			if (projectile.direction == 0)
				projectile.spriteDirection = projectile.direction = owner.direction;

			float distance = 400f;
			bool hasTarget = false;
			Vector2 targetPos = projectile.position;

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy() && Collision.CanHitLine(projectile.Center, 1, 1, targetPos, 1, 1))
				{
					float currentDist = Vector2.Distance(npc.Center, projectile.Center);

					if (currentDist < distance)
					{
						hasTarget = true;
						distance = currentDist;
						targetPos = npc.Center;
					}
				}
			}

			// Set correct projectile position (above owner).
			projectile.position = owner.Center - new Vector2(projectile.width * .5f, 46);

			// If has target, update rotation correctly and projecile spawning.
			if (hasTarget)
			{
				// Set correct (sprite) direction.
				if (targetPos.X > projectile.Center.X)
					projectile.spriteDirection = projectile.direction = 1;
				else
					projectile.spriteDirection = projectile.direction = -1;

				if (projectile.ai[1] >= 60)
				{
					if (projectile.owner == Main.myPlayer && Collision.CanHitLine(projectile.Center, 1, 1, targetPos, 1, 1))
					{
						Vector2 velocity = Vector2.Normalize(targetPos - projectile.Center) * 10;
						Projectile newProj = Main.projectile[Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.Bullet, projectile.damage, 0f, Main.myPlayer)];
						newProj.timeLeft = 300;
						newProj.netUpdate = true;
						newProj.tileCollide = false;
					}
					projectile.ai[1] = 0;
				}

				projectile.rotation = (targetPos - projectile.Center).ToRotation() + (projectile.direction == -1 ? (float)System.Math.PI : 0);
			}
			else
			{
				// Set correct (sprite) direction.
				projectile.spriteDirection = projectile.direction = owner.direction;

				if (projectile.ai[1] % 50 == 0)
				{
					randomRot = 0;
					while (randomRot == 0)
						randomRot = projectile.rotation + Main.rand.NextFloat(-.3f, .3f);

					if (randomRot > .8f)
						randomRot -= (randomRot - .8f);
					else if (randomRot < -.8f)
						randomRot -= (randomRot + .8f);
				}

				projectile.rotation = MathHelper.Lerp(projectile.rotation, randomRot, .02f);
			}

			// Attack cooldown.
			projectile.ai[1] += Main.rand.Next(1, 4);

			return (false);
		}

		public override bool CanDamage() => false;

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = GetTexture(Texture);

			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, projTexture.Height * .5f);
			SpriteEffects effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, null,
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, effects, 0);
			return (false);
		}
	}
}
