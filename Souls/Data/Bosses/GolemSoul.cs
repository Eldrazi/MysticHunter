using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class GolemSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.GolemHeadFree;
		public override string soulDescription => "Summon a laser shooting Golem head.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<GolemSoulProj>())
					Main.projectile[i].Kill();

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<GolemSoulProj>(), 170 + 5 * stack, 0f, p.whoAmI, stack, 255);
			return (true);
		}
	}

	public class GolemSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.GolemHead;

		private const float maxTargetDistance = 540;
		private const float maxTargetLosingDistance = 640;

		private int Target
		{
			get { return (int)projectile.ai[1]; }
			set { projectile.ai[1] = value; }
		}
		private bool HasTarget => Target != 255;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Golem Head");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.GolemHead];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.scale = .5f;
			projectile.timeLeft *= 5;
			projectile.minionSlots = 0f;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoulNet.soulNPC == NPCID.GolemHead)
				projectile.timeLeft = 2;

			// Position the projectile correctly (above the player).
			projectile.position.X = owner.Center.X - (projectile.width * .5f);
			projectile.position.Y = owner.Center.Y - (projectile.height * .5f) + owner.gfxOffY - 50;

			projectile.rotation = 0;
			projectile.direction = projectile.spriteDirection = -owner.direction;

			if (!HasTarget)
			{
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];
						if (npc.CanBeChasedBy() && projectile.Distance(npc.Center) <= maxTargetDistance)
						{
							Target = i;
							projectile.netUpdate = true;
							break;
						}
					}
				}
				projectile.frame = 0;
			}
			else
			{
				NPC npc = Main.npc[Target];
				if (!npc.CanBeChasedBy() || projectile.Distance(npc.Center) > maxTargetLosingDistance)
				{
					Target = 255;
					projectile.netUpdate = true;
				}
				else if (projectile.owner == Main.myPlayer)
				{
					if (projectile.ai[0]++ >= 90)
					{
						Vector2 velocity = Vector2.Normalize(npc.Center - projectile.Center) * 8;

						Projectile newProj = Main.projectile[Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.EyeBeam, projectile.damage, .1f, owner.whoAmI)];
						newProj.timeLeft = 300;
						newProj.friendly = true;
						newProj.hostile = false;
						newProj.netUpdate = true;

						projectile.ai[0] = 0;
					}
				}
				projectile.frame = 1;
			}

			return (false);
		}

		public override bool CanDamage() => false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle rect = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rect, lightColor, projectile.rotation, rect.Size() / 2, projectile.scale, SpriteEffects.None, 0f);

			if (projectile.frame == 0)
				return (false);

			Color eyeColor = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, 0);
			if (projectile.frame < 2)
				Main.spriteBatch.Draw(Main.golemTexture[1], projectile.Center - Main.screenPosition + new Vector2(-12, -4), new Rectangle(0, 0, Main.golemTexture[1].Width, Main.golemTexture[1].Height / 2), eyeColor, 0f, default, .6f, SpriteEffects.None, 0f);

			return (false);
		}
	}
}
