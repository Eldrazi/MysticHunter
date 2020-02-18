using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class DungeonGuardianSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DungeonGuardian;
		public string soulDescription => "Boosts invincibility time.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		bool appliedInvincibility = false;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
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
