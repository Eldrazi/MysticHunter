﻿using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class WyvernSoul : BaseSoul
	{
		public override short soulNPC => NPCID.WyvernHead;
		public override string soulDescription => "Grants increased flight time.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			float modifier = 1.1f + (.05f * stack);

			p.wingTimeMax = (int)(p.wingTimeMax * modifier);
			return (true);
		}
	}
}
