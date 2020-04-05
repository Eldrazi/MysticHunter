using Terraria;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Items
{
	public class CursedHand : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Hand");
			Tooltip.SetDefault("Increases the chance of dropping yellow souls.");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = 3;

			item.value = Item.sellPrice(0, 10, 0, 0);

			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Yellow] += 0.05f;
		}
	}
}
