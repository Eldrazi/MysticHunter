#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.PumpkinMoon
{
	public class PoltergeistSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Poltergeist;
		public override string soulDescription => "Increases pickup range.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().poltergeistSoul = true;
			return (true);
		}
	}
}
