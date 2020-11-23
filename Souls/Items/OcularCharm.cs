using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class OcularCharm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ocular Charm");
			Tooltip.SetDefault("Grants night vision and shine effects");
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
			player.nightVision = true;
			Lighting.AddLight((int)(player.position.X + (player.width / 2)) / 16, (int)(player.position.Y + (player.height / 2)) / 16, 0.8f, 0.95f, 1f);
		}
	}
}
