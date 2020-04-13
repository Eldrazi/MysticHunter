using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class AnglerFishSoul : BaseSoul
	{
		public override short soulNPC => NPCID.DesertBeast;
		public override string soulDescription => "Grants resistance to Petrification.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			float distanceModifier = 1;



			return (true);
		}
	}
}
