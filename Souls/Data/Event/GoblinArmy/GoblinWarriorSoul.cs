using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.GoblinArmy
{
	public class GoblinWarriorSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GoblinWarrior;
		public override string soulDescription => "Increases melee damage.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.meleeDamage += (.08f * stack);
			return (true);
		}
	}
}
