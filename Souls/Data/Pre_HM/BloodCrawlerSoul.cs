using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class BloodCrawlerSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.BloodCrawler;
		public override string soulDescription => "Summon flesh ripping teeth.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(10 + 3 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<BloodCrawlerSoulProj>(), 20 + stack, .1f, p.whoAmI, stack);
			return (true);
		}
	}

	public class BloodCrawlerSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Teeth");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 48;

			projectile.timeLeft = 15;
			projectile.penetrate = -1;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				Main.PlaySound(SoundID.NPCHit20, owner.Center);
			}

			// Set the correct projectile direction.
			projectile.spriteDirection = -owner.direction;

			// Set the correct projectile position.
			projectile.position.X = owner.Center.X - (projectile.width * .5f) + (40 * owner.direction);
			projectile.position.Y = owner.Center.Y - (projectile.height * .5f) + owner.gfxOffY;

			if (projectile.frameCounter++ >= 3)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			// Spawn blood particles.
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, projectile.direction * .2f, Main.rand.Next(3) - 1, 0, default, 1.2f);

			// Spawn blood projectiles, which the player can pick up.
			if (Main.netMode != NetmodeID.MultiplayerClient && target.type != NPCID.TargetDummy)
			{
				int minAmount = 1;
				int maxAmount = 2;

				if (projectile.ai[0] >= 5)
				{
					minAmount++;
					maxAmount++;
				}
				if (projectile.ai[0] >= 9)
				{
					minAmount++;
					maxAmount++;
				}

				int randAmount = Main.rand.Next(minAmount, maxAmount + 1);

				for (int i = 0; i < randAmount; ++i)
				{
					Projectile.NewProjectile(projectile.Center, new Vector2(projectile.spriteDirection * -3, -Main.rand.Next(1, 4)),
						ProjectileType<BloodCrawlerSoulProjBlood>(), 0, 0, projectile.owner);
				}
			}
		}
	}

	public class BloodCrawlerSoulProjBlood : ModProjectile
	{
		int randFrameY;

		public override string Texture => "Terraria/Dust";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;

			projectile.penetrate = 1;
			projectile.friendly = true;

			// This does not need to be net synced, since it's purely visual.
			if (Main.rand != null)
				randFrameY = Main.rand.Next(3);
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			// Make sure the owner of the projectile can pick up the healing blood blobs.
			if (owner.whoAmI == Main.myPlayer && owner.Hitbox.Intersects(projectile.Hitbox))
			{
				owner.statLife += 5;
				owner.HealEffect(5);
				projectile.Kill();
			}

			// Setup velocity changes.
			projectile.velocity.X *= .98f;
			projectile.velocity.Y += .2f;

			// Update projectile rotation.
			projectile.rotation += projectile.velocity.X * .1f;

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.X *= .9f;
			}

			return (false);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 projOrigin = new Vector2(5, 5);
			Texture2D projTexture = GetTexture(Texture);

			Rectangle drawRect = new Rectangle(50, randFrameY * 10, 10, 10);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, drawRect, lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}
	}
}
