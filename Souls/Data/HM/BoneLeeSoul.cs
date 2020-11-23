using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class BoneLeeSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.BoneLee;
		public override string soulDescription => "Occasionally dodge attacks";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			p.GetModPlayer<SoulPlayer>().preHurtModifier += OnHitModifier;
			return (true);
		}

		private bool OnHitModifier(Player player, ref int damage, PlayerDeathReason damageSource, byte soulStack)
		{
			int randomMax = (200 - (100 / 9) * soulStack);

			if (Main.rand.Next(randomMax) == 0)
			{
				player.immune = true;
				player.immuneTime = 30;
				return (false);
			}
			return (true);
		}
	}
}
