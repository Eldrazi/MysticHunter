#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class LamiaSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.DesertLamiaDark;
		public override string soulDescription => "Transform into a Lamia.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.ZoneDesert)
			{
				p.statDefense += 5 * stack;
				p.moveSpeed += .25f * stack;
				p.allDamage += .1f * stack;

				p.GetModPlayer<SoulPlayer>().lamiaSoul = true;
			}
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.DesertLamiaLight };
	}
}
