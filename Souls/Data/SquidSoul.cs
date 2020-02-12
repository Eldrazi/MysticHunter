using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class SquidSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Squid;
		public string soulName => "Squid";
		public string soulDescription => "Grants extra mobility in water.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.wet)
				p.moveSpeed += .2f;
			return (true);
		}
	}
}
