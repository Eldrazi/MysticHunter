using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event
{
	public class GoblinArcherSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GoblinArcher;
		public string soulDescription => "Increases ranged damage.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.rangedDamage += (.08f * stack);
			return (true);
		}
	}
}
