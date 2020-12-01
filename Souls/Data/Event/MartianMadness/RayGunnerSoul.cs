using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class RayGunnerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.RayGunner;
		public override string soulDescription => "Grants resistance to electrified.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (stack < 9)
			{
				for (int i = 0; i < p.buffType.Length; ++i)
				{
					if (p.buffType[i] == BuffID.Electrified)
					{
						p.buffTime[i]--;
					}
				}
			}
			else
			{
				p.buffImmune[BuffID.Electrified] = true;
			}

			return (true);
		}
	}
}
