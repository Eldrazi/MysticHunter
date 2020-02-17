using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class BabySlimeSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.BabySlime;
		public string soulDescription => "Makes slime less aggressive.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			p.npcTypeNoAggro[NPCID.BlueSlime] = true;
			p.npcTypeNoAggro[NPCID.LavaSlime] = true;
			p.npcTypeNoAggro[NPCID.IceSlime] = true;
			p.npcTypeNoAggro[NPCID.SlimeMasked] = true;
			p.npcTypeNoAggro[NPCID.UmbrellaSlime] = true;
			p.npcTypeNoAggro[NPCID.SlimeRibbonRed] = true;
			p.npcTypeNoAggro[NPCID.SlimeRibbonWhite] = true;
			p.npcTypeNoAggro[NPCID.SlimeRibbonGreen] = true;
			p.npcTypeNoAggro[NPCID.SlimeRibbonYellow] = true;
			if (stack >= 5)
			{
				p.npcTypeNoAggro[NPCID.Slimer] = true;
				p.npcTypeNoAggro[NPCID.Crimslime] = true;
				p.npcTypeNoAggro[NPCID.SandSlime] = true;
				p.npcTypeNoAggro[NPCID.MotherSlime] = true;
				p.npcTypeNoAggro[NPCID.SlimeSpiked] = true;
				p.npcTypeNoAggro[NPCID.DungeonSlime] = true;
				p.npcTypeNoAggro[NPCID.CorruptSlime] = true;
				p.npcTypeNoAggro[NPCID.SpikedIceSlime] = true;
				p.npcTypeNoAggro[NPCID.SpikedJungleSlime] = true;
			}
			if (stack >= 9)
			{
				p.npcTypeNoAggro[NPCID.Gastropod] = true;
				p.npcTypeNoAggro[NPCID.ToxicSludge] = true;
				p.npcTypeNoAggro[NPCID.RainbowSlime] = true;
				p.npcTypeNoAggro[NPCID.IlluminantSlime] = true;
			}
			return (true);
		}
	}
}
