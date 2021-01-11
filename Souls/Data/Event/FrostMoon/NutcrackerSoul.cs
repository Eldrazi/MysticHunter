#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Event.FrostMoon
{
	public class NutcrackerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.Nutcracker;
		public override string soulDescription => "Use health restore mana with nuts.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			int lifeCost = 10 + 5 * stack;
			int manaRegain = 5 + 5 * stack;

			if (p.statLife - lifeCost <= 0)
			{
				return (false);
			}

			if (p.statMana + manaRegain > p.statManaMax2)
			{
				if (p.statMana == p.statManaMax2)
				{
					return (false);
				}
				manaRegain = p.statManaMax2 - p.statMana;
			}

			p.statMana += manaRegain;
			p.ManaEffect(manaRegain);

			p.Hurt(PlayerDeathReason.ByCustomReason("Ate too much..."), lifeCost, 0);
			SoundEngine.PlaySound(SoundID.Item2, p.position);

			return (true);
		}

		public override short[] GetAdditionalTypes() => new short[] { NPCID.NutcrackerSpinning };
	}
}
