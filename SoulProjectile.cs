using Terraria;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

namespace MysticHunter
{
	public class SoulProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		private bool justSpawned;
		public override void SetDefaults(Projectile projectile)
		{
			justSpawned = true;
		}

		public override bool PreAI(Projectile projectile)
		{
			if (justSpawned)
			{
				SoulPlayer sp = Main.player[projectile.owner].GetModPlayer<SoulPlayer>();
				if (sp.ghostSoul && projectile.penetrate != -1)
				{
					int additivePenetration = 1;
					if (sp.activeSouls[(int)SoulType.Yellow].stack >= 5)
						additivePenetration++;
					if (sp.activeSouls[(int)SoulType.Yellow].stack >= 9)
						additivePenetration++;

					projectile.penetrate = projectile.maxPenetrate = (projectile.penetrate + additivePenetration);
				}
				justSpawned = false;
			}
			return (true);
		}
	}
}
