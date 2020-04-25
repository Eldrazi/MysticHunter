using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class VoodooDemonSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.VoodooDemon;
		public override string soulDescription => "Increases magic damage at the cost of defense.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.magicDamage += .1f * stack;
			return (true);
		}
	}
}
