using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class LihzahrdSoul : BaseSoul
	{
		public override short soulNPC => NPCID.Lihzahrd;
		public override string soulDescription => "Massive buff to all stats in temple.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			bool inTemple = false;

			int minX = (int)p.position.X / 16;
			int maxX = (int)(p.position.X + p.width) / 16;
			int minY = (int)p.position.Y / 16;
			int maxY = (int)(p.position.Y + p.height) / 16;

			for (int x = minX; x <= maxX; ++x)
			{
				for (int y = minY; y <= maxY; ++y)
				{
					if (Main.tile[x, y].wall == WallID.LihzahrdBrickUnsafe)
					{
						inTemple = true;
						break;
					}
				}
			}

			if (inTemple)
			{
				p.statDefense += 10 * stack;
				p.meleeDamage += .1f * stack;
				p.magicDamage += .1f * stack;
				p.rangedDamage += .1f * stack;
				p.minionDamage += .1f * stack;
			}

			return (true);
		}
	}
}
