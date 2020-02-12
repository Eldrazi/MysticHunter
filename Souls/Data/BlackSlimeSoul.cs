using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class BlackSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.BlackSlime;
		public string soulName => "Black Slime";
		public string soulDescription => "Boosts stats while underground.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneRockLayerHeight)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
