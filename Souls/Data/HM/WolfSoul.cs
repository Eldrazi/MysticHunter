using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class WolfSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Wolf;
		public override string soulDescription => "Increases dash distance.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		private bool applied = false;
		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.dashDelay >= 0)
				this.applied = false;
			else if (p.dashDelay < 0 && !applied)
			{
				float velocityModifier = 1 + .05f * stack;
				p.velocity.X *= velocityModifier;

				this.applied = true;
			}
			return (true);
		}
	}
}
