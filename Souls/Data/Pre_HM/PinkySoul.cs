using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class PinkySoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Pinky;
		public override string soulDescription => "Decrease enemy spawn rate.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().pinkySoul = true;
			return (true);
		}
	}
}
