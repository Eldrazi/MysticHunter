using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class VileGhoulSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DesertGhoulCorruption;
		public string soulDescription => "Grants resistance to Cursed Fire.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			if (stack >= 9)
				p.buffImmune[BuffID.CursedInferno] = true;

			for (int i = 0; i < p.buffType.Length; ++i)
			{
				if (p.buffType[i] == BuffID.CursedInferno && p.buffTime[i] >= 2)
				{
					p.buffTime[i]--;
					if (stack >= 5)
						p.buffTime[i]--;
				}
			}
			return (true);
		}
	}
}
