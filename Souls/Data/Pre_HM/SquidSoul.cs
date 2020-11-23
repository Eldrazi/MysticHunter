using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SquidSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Squid;
		public override string soulDescription => "Grants extra mobility in water.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.wet)
				p.moveSpeed += .1f * stack;
			return (true);
		}
	}
}
