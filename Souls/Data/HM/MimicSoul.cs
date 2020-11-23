using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class MimicSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Mimic;
		public override string soulDescription => "Grants increased gold drops.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack) => true;
		public override void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack)
			=> npc.AddBuff(BuffID.Midas, 30 * stack);
	}
}
