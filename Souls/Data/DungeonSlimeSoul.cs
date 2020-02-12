using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class DungeonSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DungeonSlime;
		public string soulName => "Dungeon Slime";
		public string soulDescription => "Boosts stats while in the dungeon.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneDungeon)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
