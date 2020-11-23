using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using System.IO;
using System;

namespace MysticHunter.Souls.Data.Bosses
{
	public class PlanteraSoul : PreHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.Plantera;
		public override string soulDescription => "Pull your enemies close.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 32;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 4;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<PlanteraSoulProj>(), 25 + stack, .2f, p.whoAmI);
			return (true);
		}
	}

	public class PlanteraSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.PlanterasHook;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plant Hook");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 36;

			projectile.scale = .6f;
			projectile.penetrate = -1;
			projectile.timeLeft = 600;

			projectile.melee = true;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (!owner.active || owner.dead)
				projectile.Kill();

			if (projectile.ai[0] == 0)
			{
				projectile.ai[1] = 255;

				// Animation.
				if (projectile.frameCounter++ >= 5)
				{
					projectile.frameCounter = 0;
					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				}

				if (projectile.timeLeft <= 120 || projectile.Distance(owner.Center) >= 600)
					projectile.ai[0] = 1;
			}
			else
			{
				projectile.tileCollide = false;

				if (projectile.ai[1] != 255)
				{
					NPC target = Main.npc[(int)projectile.ai[1]];

					if (!target.CanBeChasedBy(projectile))
						projectile.ai[1] = 255;
					else
						target.Center = projectile.Center;

					// Set correct velocity.
					projectile.localAI[0] += .2f;
					projectile.velocity = Vector2.Normalize(owner.Center - projectile.Center) * (projectile.localAI[0] % 5);

					// Animation.
					projectile.frame = 0;
				}
				else
				{
					projectile.velocity = Vector2.Normalize(owner.Center - projectile.Center) * 6;

					// Animation.
					if (projectile.frameCounter++ >= 5)
					{
						projectile.frameCounter = 0;
						projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
					}
				}

				// Check distance to owner and despawn.
				if (projectile.Distance(owner.Center) <= 48)
					projectile.Kill();
			}

			// Set correct rotation towards owner.
			projectile.rotation = (owner.Center - projectile.Center).ToRotation() - MathHelper.PiOver2;

			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = true;
			return (true);
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.ai[0] = 1;
			projectile.netUpdate = true;
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.ai[0] != 0 || !target.CanBeChasedBy(projectile) || target.boss)
				return;
			projectile.ai[0] = 1;
			projectile.damage /= 2;
			projectile.netUpdate = true;
			projectile.ai[1] = target.whoAmI;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Grass, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			// Draw chain.
			Vector2 chainDrawPos = projectile.Center;
			Vector2 dirToOwner = Main.player[projectile.owner].Center - chainDrawPos;
			float rot = dirToOwner.ToRotation() - MathHelper.PiOver2;
			bool drawChain = true;

			while (drawChain)
			{
				int rectHeight = (int)(16 * projectile.scale);
				int chainHeight = (int)(32 * projectile.scale);
				float length = dirToOwner.Length();
				if (length < chainHeight)
				{
					rectHeight = (int)length - chainHeight + rectHeight;
					drawChain = false;
				}
				length = rectHeight / length;
				dirToOwner *= length;
				chainDrawPos += dirToOwner;
				dirToOwner = Main.player[projectile.owner].Center - chainDrawPos;

				Color c = Lighting.GetColor((int)chainDrawPos.X / 16, (int)chainDrawPos.Y / 16);
				spriteBatch.Draw(Main.chain26Texture, chainDrawPos - Main.screenPosition, new Rectangle(0, 0, Main.chain26Texture.Width, rectHeight), c, rot,
					Main.chain26Texture.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
			}

			// Draw projectile.
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle projRect = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, projRect, lightColor, projectile.rotation, projRect.Size() / 2, projectile.scale, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
