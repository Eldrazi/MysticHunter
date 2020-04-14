using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class ArmoredVikingSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.ArmoredViking;
		public override string soulDescription => "Summons an icy axe.";

		public override short cooldown => 480;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(25 + 2 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<ArmoredVikingSoulProj>(), 30 + 5 * stack, .5f + .1f * stack, p.whoAmI);
			return (true);
		}
	}

	public class ArmoredVikingSoulProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Icy Axe");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 20;

			projectile.magic = true;
			projectile.friendly = true;

			projectile.alpha = 255;
		}

		public override bool PreAI()
		{
			Player player = Main.player[projectile.owner];

			Vector2 mountedCenter = player.Center - new Vector2(projectile.width / 2, projectile.height / 2);

			if (projectile.ai[0] == 0)
			{
				projectile.spriteDirection = projectile.direction = player.direction;
				projectile.ai[0] = 1;
				projectile.localAI[0] = 1;

				if (projectile.spriteDirection == -1)
					projectile.ai[1] = (float)Math.PI;

				projectile.scale = .4f;
			}
			else
			{
				// Wind up phase.
				if (projectile.ai[0] <= 40)
				{
					if (projectile.ai[0] <= 20)
						projectile.ai[1] -= .1f * projectile.spriteDirection;
					else if (projectile.ai[0] <= 40)
					{
						projectile.ai[1] -= (.1f * projectile.spriteDirection) * projectile.localAI[0];
						projectile.localAI[0] -= .03f;
					}

					if (projectile.scale < 1f)
						projectile.scale += .02f;
					else
						projectile.scale = 1f;
				}
				else
				{
					projectile.ai[1] += .2f * projectile.spriteDirection;

					if (projectile.ai[0] >= 80)
						projectile.scale -= .02f;
					if (projectile.ai[0] >= 96)
						projectile.Kill();
				}

				projectile.ai[0]++;
			}

			projectile.rotation = projectile.ai[1] + ((projectile.spriteDirection == 1) ? MathHelper.PiOver4 : -((float)Math.PI * 1.25f));
			projectile.position = mountedCenter + new Vector2((float)Math.Cos(projectile.ai[1]), (float)Math.Sin(projectile.ai[1])) * (40 * projectile.scale);

			if (projectile.alpha > 0)
				projectile.alpha -= 30;
			else
				projectile.alpha = 0;

			return (false);
		}
	}
}
