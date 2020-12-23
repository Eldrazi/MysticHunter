#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostMoon
{
	public class IceQueenSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.IceQueen;
		public override string soulDescription => "Slow enemies on hit.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack) => true;

		public override void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack)
		{
			if (hitEntity is Item)
			{
				npc.AddBuff(BuffID.Slow, 180);
			}
		}
	}
}
