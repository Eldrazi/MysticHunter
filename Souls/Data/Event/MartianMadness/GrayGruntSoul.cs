#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class GrayGruntSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GrayGrunt;
		public override string soulDescription => "Increases all stats during alien invasions.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.invasionType != InvasionID.MartianMadness)
			{
				return (false);
			}

			p.statDefense += 5 * stack;

			p.GetCrit<Magic>() += 5;
			p.GetCrit<Melee>() += 5;
			p.GetCrit<Ranged>() += 5;
			p.GetCrit<Summon>() += 5;
			p.GetCrit<Throwing>() += 5;

			p.allDamage += (.05f * stack);

			return (true);
		}
	}
}
