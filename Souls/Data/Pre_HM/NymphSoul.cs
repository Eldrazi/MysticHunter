using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class NymphSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Nymph;
		public override string soulDescription => "Grants spelunker effects.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.AddBuff(BuffID.Spelunker, 10);
			return (true);
		}
	}
}
