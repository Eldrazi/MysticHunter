﻿using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class LunarRitual : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lunar Ritual");
			Tooltip.SetDefault("Inflict daybroken on melee hit");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = 3;

			item.value = Item.sellPrice(0, 2, 0, 0);

			item.material = true;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SoulPlayer>().LunarRitual = true;
		}
	}
}
