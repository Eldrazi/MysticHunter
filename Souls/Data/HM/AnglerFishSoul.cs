using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class AnglerFishSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.AnglerFish;
		public override string soulDescription => "Boast an attractive lure.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			float modifier = .9f + .1f * stack;

			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				if (!Main.npc[i].active || p.Distance(Main.npc[i].Center) > 64 * modifier) continue;

				NPC npc = Main.npc[i];

				npc.defense = npc.defDefense - (int)(10 * modifier);

				if (Main.rand.Next(10) == 0)
					Dust.NewDust(npc.position, npc.width, npc.height, DustID.AncientLight);
			}

			return (true);
		}
	}
}
