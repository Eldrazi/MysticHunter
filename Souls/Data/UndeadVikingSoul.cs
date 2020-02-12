using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class UndeadVikingSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.UndeadViking;
		public string soulName => "Undead Viking";
		public string soulDescription => "Increases melee damage at the cost of defense.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			p.meleeDamage += .1f;
			p.statDefense -= 5;
			return (true);
		}
	}
}
