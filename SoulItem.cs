using Terraria;
using Terraria.ModLoader;

namespace MysticHunter
{
	public class SoulItem : GlobalItem
	{
		public override void ExtractinatorUse(int extractType, ref int resultType, ref int resultStack)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.undeadMinerSoul)
			{
				int amount = 1;
				int stack = sp.UnlockedSouls[sp.YellowSoul.soulNPC];

				if (stack >= 5)
					amount++;
				if (stack >= 9)
					amount++;

				resultStack += amount;
			}
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.poltergeistSoul == true)
			{
				grabRange += sp.YellowSoulNet.stack * 3;
			}
		}
	}
}
