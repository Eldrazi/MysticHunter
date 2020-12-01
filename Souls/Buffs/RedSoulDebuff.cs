using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Buffs
{
	public class RedSoulDebuff : ModBuff
	{
		public override void SetDefaults()
		{
			canBeCleared = false;
			Main.debuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;

			this.DisplayName.SetDefault("Red Soul Cooldown");
		}
	}
}
