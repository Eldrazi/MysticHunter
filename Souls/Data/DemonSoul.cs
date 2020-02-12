using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class DemonSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Demon;
		public string soulName => "Demon";
		public string soulDescription => "Boosts stats while in the underworld.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneUnderworldHeight)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
