using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class NymphSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Nymph;
		public string soulDescription => "Grants spelunker effects.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.AddBuff(BuffID.Spelunker, 10);
			return (true);
		}
	}
}
