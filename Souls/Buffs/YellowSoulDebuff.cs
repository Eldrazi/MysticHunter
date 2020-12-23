﻿#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace MysticHunter.Souls.Buffs
{
	public class YellowSoulDebuff : ModBuff
	{
		public override void SetDefaults()
		{
			canBeCleared = false;
			Main.debuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;

			this.DisplayName.SetDefault("Yellow Soul Cooldown");
		}
	}
}
