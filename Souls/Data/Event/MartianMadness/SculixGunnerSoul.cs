#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class ScutlixGunnerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.ScutlixRider;
		public override string soulDescription => "Grants increased damage while mounted.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (!p.mount.Active)
			{
				return (false);
			}

			p.allDamage += (.05f * stack);

			return (true);
		}
	}
}
