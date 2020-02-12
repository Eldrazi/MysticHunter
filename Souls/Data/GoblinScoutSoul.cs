using Terraria;
using Terraria.ID;
using Terraria.GameContent.Events;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class GoblinScoutSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GoblinScout;
		public string soulName => "Goblin Scout";
		public string soulDescription => "Increased chance for a goblin invasion.";

		public short cooldown => 0;
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (Main.time == 0.0)
			{
				if (!Main.snowMoon && !Main.pumpkinMoon && !DD2Event.Ongoing)
				{
					if (Main.rand.Next(60) == 0)
						Main.StartInvasion();
				}
			}
			return (true);
		}
	}
}
