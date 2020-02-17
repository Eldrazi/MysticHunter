using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class DarkMageSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DD2DarkMageT1;
		public string soulDescription => "Increases summon damage at the cost of defense.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.minionDamage += .1f * stack;
			return (true);
		}
	}
}
