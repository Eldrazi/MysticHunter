using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	[AutoloadEquip(EquipType.Neck)]
	public class MechanoScarf : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mechano Scarf");
			Tooltip.SetDefault("27% damage reduction");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = 3;

			item.value = Item.sellPrice(0, 2, 0, 0);

			item.defense = 5;
			item.material = true;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.endurance += .27f;
		}
	}
}
