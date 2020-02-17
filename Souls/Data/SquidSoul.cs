using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class SquidSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Squid;
		public string soulDescription => "Grants extra mobility in water.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			if (p.wet)
				p.moveSpeed += .1f * stack;
			return (true);
		}
	}
}
