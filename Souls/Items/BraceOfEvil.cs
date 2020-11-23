using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class BraceOfEvil : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brace of Evil");
			Tooltip.SetDefault("15% chance to inflict ichor or cursed fire on hit");
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
			player.GetModPlayer<SoulPlayer>().BraceOfEvil = true;
		}
	}
}
