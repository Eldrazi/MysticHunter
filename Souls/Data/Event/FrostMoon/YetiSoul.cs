#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostMoon
{
	public class YetiSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Yeti;
		public override string soulDescription => "Increases the effects of Frostburn.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().yetiSoul = true;
			return (true);
		}
	}
}
