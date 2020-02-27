using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event
{
	public class GoblinThiefSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GoblinThief;
		public string soulDescription => "Increases throwing damage.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.thrownDamage += (.08f * stack);
			return (true);
		}
	}
}
