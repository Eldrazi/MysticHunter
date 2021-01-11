#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class CreatureFromTheDeepSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.CreatureFromTheDeep;
		public override string soulDescription => "Increases all stats while underwater.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.wet)
			{
				p.statDefense += 5 * stack;
				p.moveSpeed += 0.1f * stack;
				p.allDamage += 0.05f * stack;
			}

			if (stack >= 5)
			{
				p.accFlipper = true;
			}
			if (stack >= 9)
			{
				p.ignoreWater = true;
				p.accDivingHelm = true;
			}

			return (true);
		}
	}
}
