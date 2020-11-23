using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class WallCreeperSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.WallCreeper;
		public override string soulDescription => "Fires an entangling web.";

		public override short cooldown => 360;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 7;

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<WallCreeperProj>(), 5 + stack, .1f, p.whoAmI);
			return (true);
		}
	}

	public class WallCreeperProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_506";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Web");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 10;

			projectile.timeLeft = 400;
			projectile.penetrate = -1;

			projectile.magic = true;
			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0]++ > 30f)
			{
				projectile.velocity.Y = projectile.velocity.Y + .2f;
				projectile.velocity.X = projectile.velocity.X * .985f;
				if (projectile.velocity.Y > 14)
					projectile.velocity.Y = 14;
			}
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * projectile.direction * .02f;
			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!target.boss)
				target.AddBuff(BuffType<WallCreeperSoulBuff>(), 300);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 4; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.Center, projectile.width, projectile.height, 30)];
				d.noGravity = true;
				d.velocity *= 0.3f;
			}
		}
	}

	public class WallCreeperSoulBuff : ModBuff
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			// NPC only buff so we'll just assign it a useless buff icon.
			texture = "Terraria/Buff";
			return base.Autoload(ref name, ref texture);
		}

		public override void SetDefaults()
		{
			DisplayName.SetDefault("Webbed");
			Description.SetDefault("Slowed");
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.velocity *= .6f;
		}
	}
}
