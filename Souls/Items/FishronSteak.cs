using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class FishronSteak : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fishron Steak");
			Tooltip.SetDefault("Grants Well Fed buff\nGrants increased abilities in water");
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
			player.wellFed = true;
			player.accFlipper = true;
			player.waterWalk2 = true;
			player.ignoreWater = true;

			if (player.wet)
			{
				player.moveSpeed += .2f;
				player.allDamage += .1f;
			}
		}
	}
}
