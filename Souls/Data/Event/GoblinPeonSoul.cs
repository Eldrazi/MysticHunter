using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event
{
	public class GoblinPeonSoul : BaseSoul
	{
		

		public override short soulNPC => NPCID.GoblinPeon;
		public override string soulDescription => "Increases movespeed.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.moveSpeed += (.08f * stack);
			return (true);
		}
	}
}
