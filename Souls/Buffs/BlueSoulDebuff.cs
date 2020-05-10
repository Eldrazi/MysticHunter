using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Buffs
{
	public class BlueSoulDebuff : ModBuff
	{
		public override void SetDefaults()
		{
			canBeCleared = false;
			Main.debuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;

			this.DisplayName.SetDefault("Blue Soul Cooldown");
		}

		public override void Update(Player player, ref int buffIndex)
		{
		}
	}
}
