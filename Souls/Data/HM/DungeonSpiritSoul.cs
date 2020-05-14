using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class DungeonSpiritSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DungeonSpirit;
		public override string soulDescription => "Periodical ectoplasm yield.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		private int currentCooldown = -1;
		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.whoAmI == Main.myPlayer && p.ZoneDungeon)
			{
				if (currentCooldown == -1)
					currentCooldown = Main.rand.Next(3200, 6401);
				if (currentCooldown-- == 0)
				{
					Item.NewItem(p.position, ItemID.Ectoplasm, Main.rand.Next(1, 3), true);
					currentCooldown = Main.rand.Next(3200, 6401);
				}
			}
			return (true);
		}
	}
}
