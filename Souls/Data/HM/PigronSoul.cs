#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class PigronSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.PigronHallow;
		public override string soulDescription => "Increased food effects.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.wellFed)
			{
				p.allDamage += 0.05f * stack;

				p.GetCrit<Melee>() += stack;
				p.GetCrit<Magic>() += stack;
				p.GetCrit<Ranged>() += stack;
				p.GetCrit<Throwing>() += stack;
				p.GetCrit<Summon>() += stack;
				
				p.minionKB += 0.2f * stack;
				p.moveSpeed += 0.1f * stack;
				p.meleeSpeed += 0.05f * stack;
				p.statDefense += (int)(1.5 * stack);
			}

			if (p.HasBuff(BuffID.Tipsy))
			{
				p.statDefense -= 2 * stack;
				p.GetCrit<Melee>() += stack;
				p.meleeSpeed += 0.075f * stack;
				p.GetDamage<Melee>() += 0.075f * stack;
			}

			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.PigronCrimson, NPCID.PigronCorruption };
	}
}
