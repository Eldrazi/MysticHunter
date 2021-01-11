#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostMoon
{
	public class GingrebreadManSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.GingerbreadMan;
		public override string soulDescription => "Use mana to restore health with cookies.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(10 + 5 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			int lifeRegain = 5 + 5 * stack;

			if (p.statLife + lifeRegain > p.statLifeMax2)
			{
				if (p.statLife == p.statLifeMax2)
				{
					return (false);
				}
				lifeRegain = p.statLifeMax2 - p.statLife;
			}

			p.statLife += lifeRegain;
			p.HealEffect(lifeRegain);
			SoundEngine.PlaySound(SoundID.Item2, p.position);

			return (true);
		}
	}
}
