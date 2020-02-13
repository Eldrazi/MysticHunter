using Terraria;

namespace MysticHunter.Souls.Framework
{
	/// <summary>
	/// The type of a soul. Used to determine how a soul is updated internally.
	/// </summary>
	public enum SoulType
	{
		Red,
		Blue,
		Yellow
	}

	public interface ISoul
	{
		bool acquired { get; set; }

		short soulNPC { get; }
		string soulName { get; }
		string soulDescription { get; }

		short cooldown { get; }

		SoulType soulType { get; }

		short ManaCost(Player player, short stack);
		bool SoulUpdate(Player player, short stack);
	}
}
