using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class MimicSoul : BaseSoul
	{
		public override short soulNPC => NPCID.Mimic;
		public override string soulDescription => "Grants resistance to Petrification.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			return (true);
		}
	}
}
