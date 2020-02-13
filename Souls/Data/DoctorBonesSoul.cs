using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class DoctorBonesSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.DoctorBones;
		public string soulName => "Doctor Bones";
		public string soulDescription => "Boosts stats while in the jungle.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Yellow;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			if (p.ZoneJungle)
			{
				p.moveSpeed += .1f * stack;
				p.statDefense += 5 * stack;
				p.allDamageMult += .1f * stack;
			}
			return (true);
		}
	}
}
