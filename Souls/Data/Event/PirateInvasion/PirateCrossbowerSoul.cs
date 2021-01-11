#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.PirateInvasion
{
	public class PirateCrossbowerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.PirateCrossbower;
		public override string soulDescription => "Increases arrow damage.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack) => true;

		public override void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack)
		{
			if (!(hitEntity is Projectile) || ((Projectile)hitEntity).arrow == false)
			{
				return;
			}

			damage += 5 + 5 * stack;
		}
	}
}
