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

		/// <summary>
		/// CloneNewInstances set to true so every setting is carried over for each soul if it's instanced/cloned again.
		/// </summary>
		public override bool CloneNewInstances => true;

		public override void SetDefaults()
		{
			item.width = item.height = 16;
		}

		/// <summary>
		/// Generic soul update functionality.
		/// </summary>
		/// <param name="player">The player that has the soul equipped.</param>
		/// <returns></returns>
		public abstract bool SoulUpdate(Player player);
	}
}
