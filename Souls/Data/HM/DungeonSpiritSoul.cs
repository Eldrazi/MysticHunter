using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class DungeonSpiritSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DungeonSpirit;
		public override string soulDescription => "Increases ectoplasm yield.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().dungeonSpiritSoul = true;
			return (true);
		}
	}
}
