#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.GameContent.Events;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class GoblinScoutSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.GoblinScout;
		public override string soulDescription => "Increased chance for a goblin invasion.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.time == 0.0)
			{
				if (!Main.snowMoon && !Main.pumpkinMoon && !DD2Event.Ongoing)
				{
					// Start with a 1/60 chance, end up with a 1/15 chance depending on stack size.
					int randNum = 60 - 5 * stack;
					if (Main.rand.Next(randNum) == 0)
						Main.StartInvasion();
				}
			}
			return (true);
		}
	}
}
