using Terraria;
using Terraria.ID;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data
{
	public class TimSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.Tim;
		public string soulName => "Tim";
		public string soulDescription => "Teleport to a random location.";

		public short cooldown => 1200;
		public byte manaConsume => 20;

		public SoulType soulType => SoulType.Blue;

		public bool SoulUpdate(Player p)
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
