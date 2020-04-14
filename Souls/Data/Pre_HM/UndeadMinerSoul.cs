using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class UndeadMinerSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.UndeadMiner;
		public override string soulDescription => "Increase extractinator yield.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().undeadMinerSoul = true;
			return (true);
		}
	}
}
