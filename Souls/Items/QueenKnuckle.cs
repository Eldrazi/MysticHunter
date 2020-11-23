using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class QueenKnuckle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Queen Knuckle");
			Tooltip.SetDefault("Poisons on hit\n15% chance to envenom on hit");
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
			player.GetModPlayer<SoulPlayer>().QueenKnuckle = true;
		}
	}
}
