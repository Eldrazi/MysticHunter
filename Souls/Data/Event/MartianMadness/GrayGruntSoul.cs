using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class GrayGruntSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GrayGrunt;
		public override string soulDescription => "Increases all stats during alien invasions.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.invasionType != InvasionID.MartianMadness)
			{
				return (false);
			}

			p.statDefense += 5 * stack;

			p.magicCrit += 5;
			p.meleeCrit += 5;
			p.rangedCrit += 5;
			p.thrownCrit += 5;

			p.meleeDamage += (.05f * stack);
			p.magicDamage += (.05f * stack);
			p.rangedDamage += (.05f * stack);
			p.minionDamage += (.05f * stack);
			p.thrownDamage += (.05f * stack);

			return (true);
		}
	}
}
