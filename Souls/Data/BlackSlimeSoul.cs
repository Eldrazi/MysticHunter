﻿using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class BlackSlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.BlackSlime;
		public string soulDescription => "Boosts stats while underground.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			if (p.ZoneRockLayerHeight)
			{
				p.moveSpeed += .1f * stack;
				p.statDefense += 5 * stack;
				p.allDamageMult += .1f * stack;
			}
			return (true);
		}
	}
}
