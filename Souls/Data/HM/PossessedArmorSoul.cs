#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class PossessedArmorSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.PossessedArmor;
		public override string soulDescription => "Creates a set of armor.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.endurance += .2f;
			p.statDefense += 30;
			p.GetModPlayer<SoulPlayer>().possessedArmorSoul = true;
			return (true);
		}
	}
}
