using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class RaggedCasterSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.RaggedCaster;
		public override string soulDescription => "Grants resistance to Blackout.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (stack >= 9)
			{
				p.buffImmune[BuffID.Blackout] = true;
				return (true);
			}

			for (int i = 0; i < p.buffType.Length; ++i)
			{
				if (p.buffType[i] == BuffID.Blackout && p.buffTime[i] >= 2)
				{
					p.buffTime[i]--;
					if (stack >= 5)
						p.buffTime[i]--;
				}
			}
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.RaggedCasterOpenCoat };
	}
}
