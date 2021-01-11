#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Bosses
{
	public class WallOfFleshSoul : PreHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.WallofFlesh;
		public override string soulDescription => "Summon devouring hungries.";

		public override short cooldown => 60;//1800

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<WallOfFleshSoulProj>())
					Main.projectile[i].Kill();
			}

			int amount = 3;
			if (stack >= 5)
				amount += 2;
			if (stack >= 9)
				amount += 2;
			
			for (int i = 0; i < amount; ++i)
			{
				Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center).RotatedByRandom(.2f) * 3f;
				Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<WallOfFleshSoulProj>(), 30 + (2 * stack), .2f, p.whoAmI, -1);
			}
			return (true);
		}
	}

	public class WallOfFleshSoulProj : ModProjectile
	{
		private readonly float maxRange = 1000;

		public override string Texture => "Terraria/Images/NPC_" + NPCID.TheHungryII;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hungry");
			Main.projFrames[projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.scale = .6f;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			float speed = 6f;
			float acceleration = .06f;

			if (projectile.ai[0] >= 0)
			{
				NPC target = Main.npc[(int)projectile.ai[0]];
				if (!target.CanBeChasedBy(projectile) || owner.Distance(target.Center) > maxRange)
				{
					projectile.ai[0] = -1;
					projectile.netUpdate = true;
				}
				else
				{
					if (projectile.position.X < target.position.X)
					{
						projectile.velocity.X += acceleration;
						if (projectile.velocity.X < 0)
							projectile.velocity.X *= .98f;
					}
					else
					{
						projectile.velocity.X -= acceleration;
						if (projectile.velocity.X > 0)
							projectile.velocity.X *= .98f;
					}

					if (projectile.position.Y < target.position.Y)
					{
						projectile.velocity.Y += acceleration;
						if (projectile.velocity.Y < 0)
							projectile.velocity.Y *= .98f;
					}
					else
					{
						projectile.velocity.Y -= acceleration;
						if (projectile.velocity.Y > 0)
							projectile.velocity.Y *= .98f;
					}

					projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -speed, speed);
					projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -speed, speed);
				}
			}

			if (projectile.ai[0] == -1)
			{
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy(projectile) && owner.Distance(Main.npc[i].Center) <= maxRange)
					{
						projectile.ai[0] = i;
						projectile.netUpdate = true;
						break;
					}
				}

				if (projectile.velocity.Length() > speed / 2)
					projectile.velocity *= .95f;
			}

			// Set correct projectile rotation.
			projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI;

			// Animate projectile.
			if (projectile.frameCounter++ >= 6)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player owner = Main.player[projectile.owner];

			int healAmount = damage / 10;
			owner.HealEffect(healAmount);
			owner.statLife += healAmount;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = TextureAssets.Projectile[Type].Value;

			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, (projTexture.Height / Main.projFrames[projectile.type]) * .5f);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(projTexture, 1, Main.projFrames[projectile.type], 0, projectile.frame)),
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);
			return (false);
		}
	}
}
