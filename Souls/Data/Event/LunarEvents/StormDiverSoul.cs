#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class StormDiverSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.VortexRifleman;
		public override string soulDescription => "Grants immunity to weird gravity.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.buffImmune[BuffID.VortexDebuff] = true;

			return (true);
		}
	}
}
