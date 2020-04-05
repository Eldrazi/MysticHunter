using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class UndeadMinerSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.UndeadMiner;
		public string soulDescription => "Increase extractinator yield.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().undeadMinerSoul = true;
			return (true);
		}
	}
}
