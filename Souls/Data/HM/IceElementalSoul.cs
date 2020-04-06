﻿using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class IceElementalSoul : BaseSoul
	{
		public override short soulNPC => NPCID.IceElemental;
		public override string soulDescription => "Grants resistance to Frozen.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (stack >= 9)
			{
				p.buffImmune[BuffID.Frozen] = true;
				p.buffImmune[BuffID.Chilled] = true;
				return (true);
			}

			for (int i = 0; i < p.buffType.Length; ++i)
			{
				if ((p.buffType[i] == BuffID.Frozen || p.buffType[i] == BuffID.Chilled) && p.buffTime[i] >= 2)
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
