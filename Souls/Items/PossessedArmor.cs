using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class PossessedArmorHeadTexture : EquipTexture
	{
		public override bool DrawHead() => true;
	}
	[Autoload(false)]
	public class PossessedArmorHead : ModItem
	{

	}

	public class PossessedArmorBodyTexture : EquipTexture
	{
		public override bool DrawBody() => false;
	}
	[Autoload(false)]
	public class PossessedArmor : ModItem
	{

	}

	public class PossessedArmorLegsTexture : EquipTexture
	{
		public override bool DrawLegs() => false;
	}
	[Autoload(false)]
	public class PossessedArmorLegs : ModItem
	{

	}
}
