using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class LuminiteRing : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminite Ring");
			Tooltip.SetDefault("It radiates energy...");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = 3;

			item.value = Item.sellPrice(0, 2, 0, 0);

			item.material = true;
			item.accessory = true;
		}
	}
}
