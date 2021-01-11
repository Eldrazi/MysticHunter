#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class UnicornSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Unicorn;
		public override string soulDescription => "Boosts stats while in hallow.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.ZoneHallow)
			{
				p.moveSpeed += .1f * stack;
				p.statDefense += 5 * stack;
				p.allDamage.multiplicative += .1f * stack;
			}
			return (true);
		}
	}
}
