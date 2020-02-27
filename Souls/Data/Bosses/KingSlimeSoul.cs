﻿using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class KingSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.KingSlime;
		public string soulDescription => "Increases throwing damage at the cost of defense.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.statDefense -= 5 * stack;
			p.thrownDamage += .1f * stack;
			return (true);
		}
	}
}
