#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.Rain
{
	public class RainbowSlimeSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.RainbowSlime;
		public override string soulDescription => "Resistance to all debuffs.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Resistance to all debuffs, halfs debuff time if not 9 stack.
			if (stack < 9)
			{
				for (int i = 0; i < p.buffType.Length; ++i)
				{
					if (p.buffType[i] == 0 || p.buffTime[i] <= 0 || !Main.debuff[p.buffType[i]])
						continue;

					p.buffTime[i]--;
				}
			}
			// If 9 stacks, complete immunity to all debuffs.
			else
			{
				for (int i = 0; i < p.buffImmune.Length; ++i)
				{
					if (!Main.debuff[i])
						continue;
					p.buffImmune[i] = true;
				}
			}
			return (true);
		}
	}
}
