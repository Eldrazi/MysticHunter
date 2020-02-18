using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class PinkySoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Pinky;
		public string soulDescription => "Decrease enemy spawn rate.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().pinkySoul = true;
			return (true);
		}
	}
}
