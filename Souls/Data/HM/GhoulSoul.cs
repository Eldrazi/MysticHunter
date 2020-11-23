using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class GhoulSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DesertGhoul;
		public override string soulDescription => "You thrive in sandstorms.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.buffImmune[BuffID.WindPushed] = true;

			if (stack > 1)
				p.moveSpeed += .1f * stack;

			return (true);
		}
	}
}
