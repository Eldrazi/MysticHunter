using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MysticHunter.Souls.Items
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

	/// <summary>
	/// The basic class for a soul item.
	/// We can use this class as a base for all other souls.
	/// </summary>
	public abstract class BasicSoul : ModItem
	{
		/// <summary>
		/// The type of this soul.
		/// Defaults to SoulType.Red.
		/// </summary>
		public SoulType soulType = SoulType.Red;

		public override void SetDefaults()
		{
			item.width = item.height = 16;

			// Remove this when the Soul Book has been implemented.
			// This is just for testing purposes.
			item.accessory = true;
		}

		// This also needs to be removed once the Soul Book is implemented.
		// This functionality will all be handled inside a ModPlayer class (active abilities as well).
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (soulType == SoulType.Yellow)
				SoulUpdate(player);
		}

		/// <summary>
		/// Generic soul update functionality.
		/// </summary>
		/// <param name="player">The player that has the soul equipped.</param>
		/// <returns></returns>
		public abstract bool SoulUpdate(Player player);
	}
}
