using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class CursedHand : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Hand");
			Tooltip.SetDefault("Increased enemy spawnrate\n10% damage reduction");
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
			player.endurance += .1f;
			player.ZoneWaterCandle = true;
		}
	}
}
