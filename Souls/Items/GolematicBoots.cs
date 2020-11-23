using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class GolematicBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Golematic Boots");
			Tooltip.SetDefault("Grants immunity to gravity effects");
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
			player.buffImmune[BuffID.Gravitation] = true;
			player.buffImmune[BuffID.VortexDebuff] = true;
		}
	}
}
