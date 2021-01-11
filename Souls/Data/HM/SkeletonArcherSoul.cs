#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;
using Terraria.ModLoader;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class SkeletonArcherSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.SkeletonArcher;
		public override string soulDescription => "Increases ranged damage at the cost of defense.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.GetDamage<Ranged>() += .1f * stack;
			return (true);
		}
	}
}
