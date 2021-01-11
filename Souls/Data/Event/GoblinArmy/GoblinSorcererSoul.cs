﻿#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.GoblinArmy
{
	public class GoblinSorcererSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GoblinSorcerer;
		public override string soulDescription => "Increases magic damage.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetDamage<Magic>() += (.08f * stack);
			return (true);
		}
	}
}
