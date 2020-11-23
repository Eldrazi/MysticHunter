using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class MartianProbeSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.MartianProbe;
		public override string soulDescription => "Summon an invasion probe.";

		public override short cooldown => 18000;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 50;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.CanStartInvasion(InvasionID.MartianMadness, true))
			{
				if (Main.netMode != 1)
					Main.StartInvasion(InvasionID.MartianMadness);
				else
					NetMessage.SendData(61, -1, -1, null, p.whoAmI, -7);
			}
			else
			{
				Main.NewText("You harness the power of the soul, but it dissipates immediately...");
			}
			return (true);
		}
	}
}
