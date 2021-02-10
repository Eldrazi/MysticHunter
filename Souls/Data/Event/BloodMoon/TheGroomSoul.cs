#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostLegion
{
	public class TheGroomSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.TheGroom;
		public override string soulDescription => "Increases damage based on the amount of nearby teammates.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxPlayers; ++i)
			{
				if (Main.player[i].active && !Main.player[i].dead && Main.player[i].team == p.team && p.team != 0)
				{
					if (p.Distance(Main.player[i].Center) < 800f)
					{
						p.allDamage += 0.2f;
					}
				}
			}

			return (true);
		}
	}
}
