#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.SolarEclipse
{
	public class BabyMothronSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MothronSpawn;
		public override string soulDescription => "Increases all stats during a Solar Eclipse.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.eclipse)
			{
				p.statDefense += 5 * stack;
				p.moveSpeed += 0.1f * stack;
				p.allDamage += 0.05f * stack;
			}

			return (true);
		}
	}
}
