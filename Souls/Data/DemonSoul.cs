using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class DemonSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Demon;
		public string soulName => "Demon";
		public string soulDescription => "Boosts stats while in the underworld.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			if (p.ZoneUnderworldHeight)
			{
				p.moveSpeed += .1f * stack;
				p.statDefense += 5 * stack;
				p.allDamageMult += .1f * stack;
			}
			return (true);
		}
	}
}
