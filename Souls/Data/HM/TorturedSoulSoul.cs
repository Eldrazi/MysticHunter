#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class TorturedSoulSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DemonTaxCollector;
		public override string soulDescription => "Allows hurting of friendly NPCs.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().torturedSoulSoul = true;
			return (true);
		}
	}
}
