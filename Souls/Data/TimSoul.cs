using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class TimSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Tim;
		public string soulDescription => "Teleport to a random location.";

		public short cooldown => 1200;

		public SoulType soulType => SoulType.Blue;

		public short ManaCost(Player p, short stack) => (short)(50 - 5 * stack);
		public bool SoulUpdate(Player p, short stack)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				p.TeleportationPotion();
			else if (Main.netMode == NetmodeID.MultiplayerClient && p.whoAmI == Main.myPlayer)
				NetMessage.SendData(73);
			Main.PlaySound(SoundID.Item6, p.Center);
			return (true);
		}
	}
}
