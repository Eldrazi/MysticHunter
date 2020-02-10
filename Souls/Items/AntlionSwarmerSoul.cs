using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class AntlionSwarmerSoul : BasicSoul
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Antlion Swarmer Soul");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			soulType = SoulType.Yellow;
		}

		public override bool SoulUpdate(Player p)
		{
			if (p.ZoneDesert || p.ZoneUndergroundDesert)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
