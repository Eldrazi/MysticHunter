using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class DarkMageSoul : BaseSoul
	{
		public override short soulNPC => NPCID.DD2DarkMageT1;
		public override string soulDescription => "Increases summon damage at the cost of defense.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.minionDamage += .1f * stack;
			return (true);
		}
	}
}
