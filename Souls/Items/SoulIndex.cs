using System.Collections.Generic;

using Terraria.ID;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class SoulIndex : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Index");

			Tooltip.SetDefault("Look through the souls of monsters you've slain.\n" +
				"[c/FF0000:Red souls] are Offensive type.\n" +
				"[c/0000FF:Blue souls] are Utility type.\n" +
				"[c/FFFF00:Yellow souls] are Enchanted type.");
		}

		public override void SetDefaults()
		{
			item.width = item.height = 16;

			item.material = false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Book);
			recipe.AddIngredient(ItemID.StoneBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
