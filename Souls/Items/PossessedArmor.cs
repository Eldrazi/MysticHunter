using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
{
	public class PossessedArmorHead : EquipTexture
	{
		public override bool DrawHead() => true;
	}

	public class PossessedArmorBody : EquipTexture
	{
		public override bool DrawBody() => false;
	}

	public class PossessedArmorLegs : EquipTexture
	{
		public override bool DrawLegs() => false;
	}
}
