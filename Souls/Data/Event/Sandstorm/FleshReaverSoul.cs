using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.Sandstorm
{
	public class FleshReaverSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SandsharkCrimson;
		public override string soulDescription => "Send out a charging Flesh Reaver.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60 + 10 * stack;
			float knockBack = 1f + .05f * stack;

			Vector2 velocity = new Vector2(Vector2.Normalize(Main.MouseWorld - p.Center).X, 0) * 6;
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<FleshReaverSoulProj>(), damage, knockBack, p.whoAmI, 0, (stack >= 9 ? 1 : 0));
			return (true);
		}
	}

	internal sealed class FleshReaverSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.SandsharkCrimson;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = 100;
			projectile.height = 24;

			projectile.scale = .8f;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;

			projectile.hide = true;
			projectile.magic = true;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.tileCollide = false;

			drawOriginOffsetY = -8;
		}

		public override bool PreAI()
		{
			Point pt = (projectile.Center + new Vector2(0, 16)).ToTileCoordinates();
			Tile tileSafely = Framing.GetTileSafely(pt);

			projectile.spriteDirection = -projectile.direction;

			bool isCollision = tileSafely.nactive() && Main.tileSolid[tileSafely.type];
			isCollision |= projectile.wet;

			if (projectile.localAI[0] == -1f && !isCollision)
			{
				projectile.localAI[0] = 20f;
			}
			if (projectile.localAI[0] > 0f)
			{
				projectile.localAI[0]--;
			}

			if (isCollision)
			{
				WorldGen.KillTile(pt.X + projectile.direction * 2, pt.Y, true, true);

				pt = (projectile.Center + new Vector2(0, 28)).ToTileCoordinates();
				tileSafely = Framing.GetTileSafely(pt.X, pt.Y - 1);

				isCollision = tileSafely.nactive() && Main.tileSolid[tileSafely.type];
				isCollision |= projectile.wet;

				projectile.ai[1] = isCollision.ToInt();
				if (projectile.ai[0] < 30)
				{
					projectile.ai[0]++;
				}

				if (isCollision)
				{
					projectile.velocity.X += projectile.direction * .15f;
					projectile.velocity.Y -= .1f;

					projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -5, 5);
					projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y, -3, 3);
				}
			}
			else
			{
				projectile.velocity.Y += .08f;
			}

			projectile.rotation = projectile.velocity.Y * projectile.direction * .1f;
			projectile.rotation = MathHelper.Clamp(projectile.rotation, -.2f, .2f);

			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			return (false);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			drawCacheProjsBehindNPCsAndTiles.Add(index);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20 * projectile.scale; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, -projectile.velocity.X * .2f, -projectile.velocity.Y * .2f, 100)];
				d.noGravity = true;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(target.position, target.width, target.height, DustID.Blood, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
			}
		}
	}
}

