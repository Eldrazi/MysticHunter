using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class DungeonGuardianSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.DungeonGuardian;
		public override string soulDescription => "Boosts invincibility time.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		bool appliedInvincibility = false;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.immuneTime == 0)
				appliedInvincibility = false;
			else if (!appliedInvincibility)
			{
				p.immuneTime += 60 + 30 * stack;
				appliedInvincibility = true;
			}
			return (true);
		}
	}
}
