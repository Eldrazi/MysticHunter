#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

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
	}
}
