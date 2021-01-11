#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.Audio;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class TimSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Tim;
		public override string soulDescription => "Teleport to a random location.";

		public override short cooldown => 1200;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(50 - 5 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				p.TeleportationPotion();
			else if (Main.netMode == NetmodeID.MultiplayerClient && p.whoAmI == Main.myPlayer)
				NetMessage.SendData(MessageID.Teleport);
			SoundEngine.PlaySound(SoundID.Item6, p.Center);
			return (true);
		}
	}
}
