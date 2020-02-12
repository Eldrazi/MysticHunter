using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class IceSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.IceSlime;
		public string soulName => "Ice Slime";
		public string soulDescription => "Boosts stats while in icy lands.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneSnow)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
