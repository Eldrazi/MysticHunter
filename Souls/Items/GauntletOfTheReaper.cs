using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Items
{
	public class GauntletOfTheReaper : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauntlet of theReaper");
			Tooltip.SetDefault("Increases the chance of dropping souls.");
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = 3;

			item.value = Item.sellPrice(0, 10, 0, 0);

			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Red] += 0.015f;
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Blue] += 0.015f;
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Yellow] += 0.015f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<BraceOfEvil>(), 1);
			recipe.AddIngredient(ItemType<CursedHand>(), 1);
			recipe.AddIngredient(ItemType<OcularCharm>(), 1);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
