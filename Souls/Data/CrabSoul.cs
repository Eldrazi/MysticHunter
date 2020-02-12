using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class CrabSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Crab;
		public string soulName => "Crab";
		public string soulDescription => "Boosts stats while at the ocean.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneBeach)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
