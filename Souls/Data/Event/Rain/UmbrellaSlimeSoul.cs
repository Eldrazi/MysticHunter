using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.Rain
{
	public class UmbrellaSlimeSoul : PreHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.UmbrellaSlime;
		public override string soulDescription => "Increases the likelihood of a rainstorm.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient || Main.raining || Main.slimeRain)
				return (false);

			int chanceBase = 86400;
			float chanceMod = 11f - (.55f * stack);

			if (Main.rand.Next((int)(chanceBase * chanceMod)) == 0)
			{
				StartRain();
			}

			return (true);
		}

		private static void StartRain()
		{
			int maxRainTime = 86400;
			int minRainTime = maxRainTime / 24;

			Main.rainTime = Main.rand.Next(minRainTime * 8, maxRainTime);

			if (Main.rand.Next(3) == 0)
			{
				Main.rainTime += Main.rand.Next(0, minRainTime);
			}
			if (Main.rand.Next(4) == 0)
			{
				Main.rainTime += Main.rand.Next(0, minRainTime * 2);
			}
			if (Main.rand.Next(5) == 0)
			{
				Main.rainTime += Main.rand.Next(0, minRainTime * 2);
			}
			if (Main.rand.Next(6) == 0)
			{
				Main.rainTime += Main.rand.Next(0, minRainTime * 3);
			}
			if (Main.rand.Next(7) == 0)
			{
				Main.rainTime += Main.rand.Next(0, minRainTime * 4);
			}
			if (Main.rand.Next(8) == 0)
			{
				Main.rainTime += Main.rand.Next(0, minRainTime * 5);
			}

			float num3 = 1f;
			if (Main.rand.Next(2) == 0)
			{
				num3 += 0.05f;
			}
			if (Main.rand.Next(3) == 0)
			{
				num3 += 0.1f;
			}
			if (Main.rand.Next(4) == 0)
			{
				num3 += 0.15f;
			}
			if (Main.rand.Next(5) == 0)
			{
				num3 += 0.2f;
			}
			Main.rainTime = (int)(Main.rainTime * num3);
			ChangeRain();
			Main.raining = true;
		}

		private static void ChangeRain()
		{
			if (Main.cloudBGActive >= 1f || Main.numClouds > 150.0)
			{
				if (Main.rand.Next(3) == 0)
				{
					Main.maxRaining = Main.rand.Next(20, 90) * 0.01f;
					return;
				}
				Main.maxRaining = Main.rand.Next(40, 90) * 0.01f;
				return;
			}
			else if (Main.numClouds > 100.0)
			{
				if (Main.rand.Next(3) == 0)
				{
					Main.maxRaining = Main.rand.Next(10, 70) * 0.01f;
					return;
				}
				Main.maxRaining = Main.rand.Next(20, 60) * 0.01f;
				return;
			}
			else
			{
				if (Main.rand.Next(3) == 0)
				{
					Main.maxRaining = Main.rand.Next(5, 40) * 0.01f;
					return;
				}
				Main.maxRaining = Main.rand.Next(5, 30) * 0.01f;
				return;
			}
		}
	}
}
