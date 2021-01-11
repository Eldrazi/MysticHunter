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

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class PinkJellyfishSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.PinkJellyfish;
		public override string soulDescription => "Summons a shocking jellyfish.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(10 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 20;
			int jellyfishSize = 1;
			if (stack >= 5)
			{
				damage += 5;
				jellyfishSize++;
			}
			if (stack >= 9)
			{
				damage += 10;
				jellyfishSize++;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 5;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<PinkJellyfishSoulProj>(), 20 + stack, .1f + .02f * stack, p.whoAmI, jellyfishSize);
			return (true);
		}
	}

	public class PinkJellyfishSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.PinkJellyfish;
		private Vector2 initialVelocity = Vector2.Zero;

		private int shockRadius { get { return (int)(projectile.ai[0] * 48); } }

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pink Jellyfish");
			Main.projFrames[projectile.type] = 7;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 26;

			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (projectile.ai[1] == 0)
			{
				projectile.ai[1] = 1;
				initialVelocity = projectile.velocity;
			}
			else if (projectile.ai[1] == 1)
			{
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy() && Vector2.Distance(projectile.Center, Main.npc[i].Center) <= shockRadius * .75f)
					{
						projectile.ai[1] = 2;
						projectile.netUpdate = true;
						break;
					}
				}

				if (projectile.frameCounter++ >= 6)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % 4;
				}

				float velocityModifier = 1;
				if (projectile.frame == 3)
					velocityModifier = .8f;
				if (projectile.frame == 0)
					velocityModifier = .6f;
				if (projectile.frame == 1)
					velocityModifier = .4f;

				// Velocity depending on current projectile frame.
				projectile.velocity = initialVelocity * velocityModifier;
			}
			else
			{
				// Slow down the projectile.
				projectile.velocity *= .95f;
				if (projectile.timeLeft > 60)
					projectile.timeLeft = 60;

				// Set shock radius.
				projectile.knockBack = 0;

				// Animate shock.
				if (projectile.frameCounter++ >= 6)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				}
				if (projectile.frame < 4)
					projectile.frame = 4;
			}

			// Set correct projectile rotation.
			projectile.rotation = (float)(Math.Atan2(projectile.velocity.Y, projectile.velocity.X)) + MathHelper.PiOver2;

			return (false);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (projectile.ai[1] == 2)
			{
				return (targetHitbox.Intersects(
					new Rectangle(
					(int)(projectile.position.X + projectile.width * .5f - shockRadius * .5f),
					(int)(projectile.position.Y + projectile.height * .5f - shockRadius * .5f),
					shockRadius, shockRadius)
				));
			}
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.ai[1] = 2;
			projectile.netUpdate = true;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PinkSlime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		int ringFrame = 0;
		int ringFrameCounter = 0;
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = TextureAssets.Projectile[Type].Value;
			Texture2D ringTexture = TextureAssets.Projectile[ProjectileID.Electrosphere].Value;
			
			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, (projTexture.Height / Main.projFrames[projectile.type]) * .5f);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(projTexture, 1, Main.projFrames[projectile.type], 0, projectile.frame)),
			lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);

			if (projectile.ai[1] == 2)
			{
				if (ringFrameCounter++ >= 6)
				{
					ringFrameCounter = 0;
					ringFrame = (ringFrame + 1) % 4;
				}

				int frameHeight = ringTexture.Height / 4;

				Vector2 ringOrigin = new Vector2(ringTexture.Width * .5f, frameHeight * .5f);

				float scale = (shockRadius / (float)ringTexture.Width);

				spriteBatch.Draw(ringTexture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(ringTexture, 1, 4, 0, ringFrame)),
					Color.Pink, 0, ringOrigin, scale, SpriteEffects.None, 0);
			}

			return (false);
		}
	}
}
