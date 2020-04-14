using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class ArmoredSkeletonSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.ArmoredSkeleton;
		public override string soulDescription => "Your attacks penetrate armor.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.armorPenetration = 5 * stack;
			return (true);
		}
	}
}
