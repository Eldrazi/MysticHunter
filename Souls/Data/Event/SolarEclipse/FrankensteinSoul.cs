#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class FrankensteinSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Frankenstein;
		public override string soulDescription => "Increases life.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statLifeMax2 += 25 * stack;

			return (true);
		}
	}
}
