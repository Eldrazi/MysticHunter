using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class FloatyGrossSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.FloatyGross;
		public string soulDescription => "Grants resistance to Weakness.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			if (stack >= 9)
			{
				p.buffImmune[BuffID.Weak] = true;
				return (true);
			}

			for (int i = 0; i < p.buffType.Length; ++i)
			{
				if (p.buffType[i] == BuffID.Weak && p.buffTime[i] >= 2)
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
