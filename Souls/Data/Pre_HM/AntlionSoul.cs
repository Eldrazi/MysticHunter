using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class AntlionSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Antlion;
		public override string soulDescription => "Summons a projectile shield.";

		public override short cooldown => 3600;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy the projectile if it already exists.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<AntlionSoulProj>())
				{
					Main.projectile[i].Kill();
					return (true);
				}
			}

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<AntlionSoulProj>(), 0, 0, p.whoAmI, 2 + stack);

			return (true);
		}
	}

	public class AntlionSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Shield");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 52;

			projectile.alpha = 120;

			projectile.hide = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoul?.soulNPC == NPCID.Antlion)
				projectile.timeLeft = 2;

			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < Main.maxProjectiles; ++i)
				{
					if (Main.projectile[i].active && Main.projectile[i].hostile && projectile.Hitbox.Intersects(Main.projectile[i].Hitbox))
						BlockProjectile(Main.projectile[i]);
				}
			}

			projectile.position = owner.Center - new Vector2(projectile.width * .5f, projectile.height * .5f);

			// Round projectile positional values to remove stuttering movement/visuals.
			projectile.position.X = (int)projectile.position.X;
			projectile.position.Y = (int)projectile.position.Y;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Sandnado, 0, 0, 100);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
			=> drawCacheProjsOverWiresUI.Add(index);

		private bool BlockProjectile(Projectile proj)
		{
			// Kill the projectile in question.
			proj.Kill();

			// Spawn extra dust flair around the killed projectile.
			for (int i = 0; i < 5; i++)
				Dust.NewDust(proj.position, proj.width, proj.height, DustID.Sandnado, proj.velocity.X * .2f, proj.velocity.Y * .2f, 100);

			// If the projectile does not have any projectile block stacks left, destroy it.
			if (--projectile.ai[0] == 0)
				projectile.Kill();

			return (true);
		}
	}
}
