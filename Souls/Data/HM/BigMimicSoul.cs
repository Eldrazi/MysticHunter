#region Using directives

using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class BigMimicSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.BigMimicCorruption;
		public override string soulDescription => "Grants increased soul droprate.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			SoulPlayer sp = p.GetModPlayer<SoulPlayer>();

			float modifier = .0025f;

			if (stack >= 3)
				modifier += 0.0025f;
			if (stack >= 6)
				modifier += 0.0025f;
			if (stack >= 9)
				modifier += 0.0025f;

			sp.soulDropModifier[0] += modifier;
			sp.soulDropModifier[1] += modifier;
			sp.soulDropModifier[2] += modifier;

			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.BigMimicCrimson, NPCID.BigMimicHallow, NPCID.BigMimicJungle };

		public override string SoulNPCName()
			=> "Big Mimic";
	}
}
