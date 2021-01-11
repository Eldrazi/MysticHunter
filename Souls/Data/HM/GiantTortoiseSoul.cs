#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class GiantTortoiseSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.GiantTortoise;
		public override string soulDescription => "Grants increased DR.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.endurance += (.05f + .05f * stack);
			return (true);
		}
	}
}
