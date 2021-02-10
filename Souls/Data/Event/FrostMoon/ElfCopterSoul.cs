#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	internal sealed class ElfCopterSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.ElfCopter;
		public override string soulDescription => "Summon an elf copter to follow you around.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 60 + 5 * stack;

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<ElfCopterSoulProj>())
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<ElfCopterSoulProj>(), damage, .5f, p.whoAmI);
			return (true);
		}
	}

	internal sealed class ElfCopterSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.ElfCopter;

		private int shootTimer = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elf Copter");
			Main.projFrames[projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.ZephyrFish);
			aiType = ProjectileID.ZephyrFish;
			projectile.scale = 0.8f;

			shootTimer = 0;
		}

		public override bool PreAI()
		{
			Player player = Main.player[projectile.owner];
			player.zephyrfish = false;
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();
			
			if (!player.dead && sp.BlueSoulNet.soulNPC == NPCID.ElfCopter)
			{
				projectile.timeLeft = 2;
			}

			if (Main.myPlayer == projectile.owner && shootTimer++ >= 120)
			{
				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy(projectile) &&
						Collision.CanHitLine(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						Vector2 newProjVelocity = Vector2.Normalize(Main.npc[i].Center - projectile.Center) * 8;
						Projectile.NewProjectile(projectile.Center, newProjVelocity, ProjectileID.Bullet, projectile.damage, 1f, projectile.owner);

						shootTimer = -20;

						return;
					}
				}

				shootTimer = 0;
			}

			if (shootTimer < 0)
			{
				if (projectile.frameCounter++ >= 5)
				{
					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				}

				if (projectile.frame < 4)
				{
					projectile.frame = 4;
				}
			}
			else
			{
				if (projectile.frame > 3)
				{
					projectile.frame = 0;
				}
			}
		}

		public override bool CanDamage() => false;
		
		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 20, 0, 0, 50);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
			Vector2 origin = frame.Size() / 2;
			SpriteEffects effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effects, 0);

			return (false);
		}
	}
}
