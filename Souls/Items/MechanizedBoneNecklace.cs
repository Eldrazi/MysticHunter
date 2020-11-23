using Terraria;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class MechanizedBoneNecklace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mechanized Bone Necklace");
			Tooltip.SetDefault("20% increased summon damage");
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
			player.minionDamage += .2f;
		}
	}
}
