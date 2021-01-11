#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class PreidctorSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.NebulaSoldier;
		public override string soulDescription => "Increases critical chance.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetCrit<Magic>() += 5 * stack;
			p.GetCrit<Melee>() += 5 * stack;
			p.GetCrit<Summon>() += 5 * stack;
			p.GetCrit<Ranged>() += 5 * stack;
			p.GetCrit<Throwing>() += 5 * stack;

			return (true);
		}
	}
}
