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
		public byte manaConsume => 0;

		public SoulType soulType => SoulType.Yellow;

		public bool SoulUpdate(Player p)
		{
			if (p.ZoneJungle)
			{
				p.statDefense += 5;
				p.allDamageMult += .1f;
			}
			return (true);
		}
	}
}
