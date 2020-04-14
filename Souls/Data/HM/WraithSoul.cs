using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class WraithSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Wraith;
		public override string soulDescription => "Allows you to pass through enemies.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			return (true);
		}

		public override void ModifyHitByNPC(Player player, NPC npc, ref int damage, ref bool crit, byte stack)
		{

		}
	}
}
