using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event
{
	public class GoblinPeonSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GoblinPeon;
		public string soulDescription => "Increases movespeed.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.moveSpeed += (.08f * stack);
			return (true);
		}
	}
}
