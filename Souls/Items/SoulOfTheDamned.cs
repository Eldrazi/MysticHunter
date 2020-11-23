using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Items
{
	public class SoulOfTheDamned : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul of the Damned");
			Tooltip.SetDefault("Increases the chance of dropping souls.");

			ItemID.Sets.ItemNoGravity[item.type] = true;
			ItemID.Sets.AnimatesAsSoul[item.type] = true;
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
			item.rare = 3;

			item.value = Item.sellPrice(0, 5, 0, 0);

			item.material = true;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Red] += 0.015f;
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Blue] += 0.015f;
			player.GetModPlayer<SoulPlayer>().soulDropModifier[(int)SoulType.Yellow] += 0.015f;
		}
	}
}
