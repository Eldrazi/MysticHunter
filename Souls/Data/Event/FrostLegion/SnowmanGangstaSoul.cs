#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	public class SnowmanGangstaSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.SnowmanGangsta;
		public override string soulDescription => "Summon a protective Snowman Gangsta.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(AnySummonAvailable(p) == -1 ? 30 : 5);
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 20 + 5 * stack;

			int i = AnySummonAvailable(p);
			if (i != -1)
			{
				Main.projectile[i].direction *= -1;
				Main.projectile[i].netUpdate = true;
				return (true);
			}

			Projectile.NewProjectile(p.Center + new Vector2(28 * p.direction, 0), new Vector2(p.direction, 0), ProjectileType<SnowmanGangstaSoulProj>(), 0, 0, p.whoAmI, 0, damage);

			return (true);
		}

		private int AnySummonAvailable(Player p)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<SnowmanGangstaSoulProj>())
				{
					return (i);
				}
			}
			return (-1);
		}
	}

	internal sealed class SnowmanGangstaSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.SnowmanGangsta;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 4;
			DisplayName.SetDefault("Snowman Gangsta");
		}
		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 40;

			projectile.minion = true;
			projectile.manualDirectionChange = true;

			projectile.timeLeft = 600;
			projectile.minionSlots = 0;

			drawOffsetX = -16;
			drawOriginOffsetY = -16;
		}

		public override bool PreAI()
		{
			if (projectile.velocity.X != 0)
			{
				Main.PlaySound(SoundID.Item10, projectile.position);
				for (int i = 0; i < (int)(20 * projectile.scale); i++)
				{
					Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 51, 0, 0, 100)];
					d.noGravity = true;
				}
				projectile.direction = Math.Sign(projectile.velocity.X);
			}
			projectile.velocity.X *= 0;
			projectile.velocity.Y += .1f;

			// Shooting.
			if (Main.myPlayer == projectile.owner && projectile.ai[0]++ >= 15)
			{
				projectile.ai[0] = 0;

				Vector2 newProjVelocity = new Vector2(-projectile.spriteDirection * 12, 1 - (Main.rand.NextFloat() * 2));
				Vector2 newProjPos = new Vector2(projectile.Center.X - projectile.direction * 12, projectile.Center.Y);
				int newProj = Projectile.NewProjectile(newProjPos, newProjVelocity, ProjectileID.BulletSnowman, (int)projectile.ai[1], .1f, projectile.whoAmI, 2f);
				Main.projectile[newProj].timeLeft = 300;
				Main.projectile[newProj].friendly = true;
				Main.projectile[newProj].hostile = false;
				NetMessage.SendData(27, -1, -1, null, newProj);
			}

			// Graphical/animation.
			if (projectile.frameCounter++ >= 15)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			if (projectile.frame == 0)
				projectile.frame = 1;
			projectile.spriteDirection = -projectile.direction;
			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item10, projectile.position);
			for (int i = 0; i < (int)(20 * projectile.scale); i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 51, 0, 0, 100)];
				d.noGravity = true;
			}
		}
	}
}
