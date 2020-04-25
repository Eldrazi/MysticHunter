using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class GraniteGolemSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.GraniteGolem;
		public override string soulDescription => "Increases base defense";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statDefense += 3 * stack;
			return (true);
		}
	}
}
