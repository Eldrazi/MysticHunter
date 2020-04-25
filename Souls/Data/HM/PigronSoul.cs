using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

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
				p.meleeDamage += 0.05f * stack;
				p.magicDamage += 0.05f * stack;
				p.rangedDamage += 0.05f * stack;
				p.thrownDamage += 0.05f * stack;
				p.minionDamage += 0.05f * stack;

				p.meleeCrit += stack;
				p.magicCrit += stack;
				p.rangedCrit += stack;
				p.thrownCrit += stack;
				
				p.minionKB += 0.2f * stack;
				p.moveSpeed += 0.1f * stack;
				p.meleeSpeed += 0.05f * stack;
				p.statDefense += (int)(1.5 * stack);
			}

			if (p.HasBuff(BuffID.Tipsy))
			{
				p.statDefense -= 2 * stack;
				p.meleeCrit += 1 * stack;
				p.meleeDamage += 0.075f * stack;
				p.meleeSpeed += 0.075f * stack;
			}

			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.PigronCrimson, NPCID.PigronCorruption };
	}
}
