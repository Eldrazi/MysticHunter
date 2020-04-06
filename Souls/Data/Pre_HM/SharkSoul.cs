using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SharkSoul : BaseSoul
	{
		public override short soulNPC => NPCID.Shark;
		public override string soulDescription => "Summons a friendly shark";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<SharkSoulProj>())
					Main.projectile[i].Kill();
			}

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<SharkSoulProj>(), 2 + stack, .1f, p.whoAmI);
			return (true);
		}
	}

	public class SharkSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_65";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shark");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;

			projectile.scale = .6f;
		}

		public override bool PreAI()
		{
			Player player = Main.player[projectile.owner];

			// Check if the projectile should still be alive.
			if (player.dead || player.GetModPlayer<SoulPlayer>().BlueSoul == null || player.GetModPlayer<SoulPlayer>().BlueSoul.soulNPC != NPCID.Shark)
				projectile.Kill();
			projectile.timeLeft = 2;

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}
	}
}
