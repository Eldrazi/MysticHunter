using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Items
{
	/// <summary>
	/// The basic class for a soul item.
	/// </summary>
	public class BasicSoul : ModItem
	{
		public short soulNPC = 0;

		public override void SetDefaults()
		{
			item.width = item.height = 16;
		}

		/// <summary>
		/// Triggers on pickup. Checks if a corresponding soul exists and sets that soul as acquired.
		/// </summary>
		/// <param name="player">The player that interacts with this item.</param>
		/// <returns>Always returns false. This item is not actually added to the players' inventory.</returns>
		public override bool OnPickup(Player player)
		{
			if (MysticHunter.Instance.SoulDict.ContainsKey(soulNPC))
			{
				ISoul s = MysticHunter.Instance.SoulDict[soulNPC];

				// If this is the first time the soul has been picked up, the SoulIndexUIList needs to be repopulated.
				if (s.acquired == false)
				{
					s.acquired = true;
					SoulManager.RepopulateSoulIndexUI();

					Color c = Color.Red;
					if (s.soulType == SoulType.Blue)
						c = Color.Blue;
					else if (s.soulType == SoulType.Yellow)
						c = Color.Yellow;
					Main.NewText("You collected the " + s.soulName + " soul.", c);
				}
			}
			else
			{
				// Debug message for when a soul with the given `soulDataID` cannot be found.
				Main.NewText("Soul with ID '" + soulNPC + "' cannot be found.");
			}

			return (false);
		}
	}
}
