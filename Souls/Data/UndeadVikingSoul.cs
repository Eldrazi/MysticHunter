using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class UndeadVikingSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.UndeadViking;
		public string soulName => "Undead Viking";
		public string soulDescription => "Increases melee damage at the cost of defense.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.meleeDamage += .1f * stack;
			return (true);
		}
	}
}
