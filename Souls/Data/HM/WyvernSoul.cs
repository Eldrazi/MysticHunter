using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class WyvernSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.WyvernHead;
		public string soulDescription => "Grants increased flight time.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			float modifier = 1.1f + (.05f * stack);

			p.wingTimeMax = (int)(p.wingTimeMax * modifier);
			return (true);
		}
	}
}
