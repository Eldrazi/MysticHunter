using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class GiantCursedSkullSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.GiantCursedSkull;
		public override string soulDescription => "Summons a projectile shield.";

		public override short cooldown => 3600;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy the projectile if it already exists.
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<GiantCursedSkullSoulProj>())
				{
					Main.projectile[i].Kill();
					return (true);
				}
			}

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<GiantCursedSkullSoulProj>(), 0, 0, p.whoAmI, 2 + stack);

			return (true);
		}
	}

	public class GiantCursedSkullSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mana Shield");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 70;

			projectile.alpha = 120;
			projectile.timeLeft = 600;

			projectile.hide = true;
			projectile.friendly = true;
			projectile.tileCollide = false;

			drawOffsetX = 2;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			// Check if the projectile should still be alive.
			if (!owner.active || owner.dead || owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Blue].soulNPC != NPCID.GiantCursedSkull)
				projectile.Kill();

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
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 16, 0, 0, 100);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
			=> drawCacheProjsOverWiresUI.Add(index);

		private bool BlockProjectile(Projectile proj)
		{
			// Kill the projectile in question.
			proj.Kill();

			// Spawn extra dust flair around the killed projectile.
			for (int i = 0; i < 5; i++)
				Dust.NewDust(proj.position, proj.width, proj.height, 16, proj.velocity.X * .2f, proj.velocity.Y * .2f, 100);

			Main.LocalPlayer.ManaEffect(10);
			Main.LocalPlayer.statMana = (int)MathHelper.Clamp(Main.LocalPlayer.statMana + 10, 0, Main.LocalPlayer.statManaMax2); 

			return (true);
		}
	}
}
