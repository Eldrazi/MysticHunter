using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.PirateInvasion
{
	public class PirateCorsairSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.PirateCorsair;
		public override string soulDescription => "Melee attacks apply the Midas debuff.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack) => true;

		public override void OnHitNPC(Player player, NPC npc, Entity hitEntity, ref int damage, byte stack)
		{
			if (!(hitEntity is Item) || ((Item)hitEntity).melee == false)
			{
				return;
			}

			if (player.whoAmI == ((Projectile)hitEntity).owner)
			{
				npc.AddBuff(BuffID.Midas, 180);
			}
		}
	}
}
