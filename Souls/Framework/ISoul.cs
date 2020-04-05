using Terraria;

namespace MysticHunter.Souls.Framework
{
	/// <summary>
	/// The type of a soul. Used to determine how a soul is updated internally.
	/// </summary>
	public enum SoulType
	{
		Red = 0,
		Blue = 1,
		Yellow = 2
	}

	public interface ISoul
	{
		bool acquired { get; set; }

		short soulNPC { get; }
		string soulDescription { get; }

		short cooldown { get; }

		SoulType soulType { get; }

		short ManaCost(Player player, short stack);
		bool SoulUpdate(Player player, short stack);
	}

	public static class ISoulExtensions
	{
		public static string SoulNPCName(this ISoul soul)
		{
			if (soul == null || soul.soulNPC == 0)
				return ("");
			return Lang.GetNPCNameValue(soul.soulNPC);
		}
	}
}
