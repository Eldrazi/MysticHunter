#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace MysticHunter.Souls.Items
{
	public class BandOfDoubleSight : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Band of double Sight");
			Tooltip.SetDefault("Grants spelunker, hunter and danger sense effects");
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
			player.dangerSense = true;
			player.findTreasure = true;
			player.detectCreature = true;
		}
	}
}
