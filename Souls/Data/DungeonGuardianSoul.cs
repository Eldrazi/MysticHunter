using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class DungeonGuardianSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DungeonGuardian;
		public string soulName => "Dungeon Guardian";
		public string soulDescription => "Boosts invincibility time.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		bool appliedInvincibility = false;

		public bool SoulUpdate(Player p)
		{
			if (p.immuneTime == 0)
				appliedInvincibility = false;
			else if (!appliedInvincibility)
			{
				p.immuneTime += 60;
				appliedInvincibility = true;
			}
			return (true);
		}
	}
}
