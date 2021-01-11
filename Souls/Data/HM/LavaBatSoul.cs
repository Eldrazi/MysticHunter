#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class LavaBatSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Lavabat;
		public override string soulDescription => "Grants resistance to lava.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (stack >= 9)
			{
				p.lavaImmune = true;
				return (true);
			}

			p.lavaTime += 60 * stack;
			return (true);
		}
	}
}
