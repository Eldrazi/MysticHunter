#region Using directives

using System;

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	public class MisterStabbySoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MisterStabby;
		public override string soulDescription => "Increase melee damage if attacking from behind.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack) => true;
		public override void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack)
		{
			int damageModifier = 5 + 5 * stack;
			// Check to see if the npc is facing away from the player.
			if (Math.Sign(player.Center.X - npc.Center.X) != npc.direction)
			{
				damage += damageModifier;
			}
		}
	}
}
