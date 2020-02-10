using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class SoulIndex : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Index");
		}

		public override void SetDefaults()
		{
			item.width = item.height = 16;
		}
	}
}
