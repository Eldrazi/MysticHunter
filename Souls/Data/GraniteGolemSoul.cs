using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class GraniteGolemSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GraniteGolem;
		public string soulName => "Granite Golem";
		public string soulDescription => "Increases base defense";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.statDefense += 3 * stack;
			return (true);
		}
	}
}
