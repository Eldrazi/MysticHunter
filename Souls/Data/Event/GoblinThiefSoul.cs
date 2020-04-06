using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event
{
	public class GoblinThiefSoul : BaseSoul
	{
		

		public override short soulNPC => NPCID.GoblinThief;
		public override string soulDescription => "Increases throwing damage.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.thrownDamage += (.08f * stack);
			return (true);
		}
	}
}
