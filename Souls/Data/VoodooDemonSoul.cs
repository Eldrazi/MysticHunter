using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class VoodooDemonSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.VoodooDemon;
		public string soulName => "Voodoo Demon";
		public string soulDescription => "Increases magic damage at the cost of defense.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			p.magicDamage += .1f;
			p.statDefense -= 5;
			return (true);
		}
	}
}
