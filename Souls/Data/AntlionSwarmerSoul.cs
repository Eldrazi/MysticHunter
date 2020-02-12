using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class AntlionSwarmerSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.FlyingAntlion;
		public string soulName => "Antlion Swarmer";
		public string soulDescription => "Boosts stats while in desert.";

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
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
