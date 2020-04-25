using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class UndeadVikingSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.UndeadViking;
		public override string soulDescription => "Increases melee damage at the cost of defense.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.meleeDamage += .1f * stack;
			return (true);
		}
	}
}
