using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class MushiLadybugSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.MushiLadybug;
		public string soulName => "Mushi Ladybug";
		public string soulDescription => "Boosts stats while in mushroom biome.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneGlowshroom)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
