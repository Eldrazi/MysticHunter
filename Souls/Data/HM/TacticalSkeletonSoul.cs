using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class TacticalSkeletonSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.TacticalSkeleton;
		public override string soulDescription => "Shoot extra bullets.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().tacticalSkeletonSoul = true;
			return (true);
		}
	}
}
