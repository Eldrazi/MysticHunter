using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.GoblinArmy
{
	public class GoblinSummonerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GoblinSummoner;
		public override string soulDescription => "Increases summon damage.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.minionDamage += (.08f * stack);
			return (true);
		}
	}
}
