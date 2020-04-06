﻿using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class KingSlimeSoul : BaseSoul
	{
		public override short soulNPC => NPCID.KingSlime;
		public override string soulDescription => "Increases throwing damage at the cost of defense.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.thrownDamage += .1f * stack;
			return (true);
		}
	}
}
