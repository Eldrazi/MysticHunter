﻿using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event
{
	public class GoblinSummonerSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GoblinSummoner;
		public string soulDescription => "Increases summon damage.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.minionDamage += (.08f * stack);
			return (true);
		}
	}
}
