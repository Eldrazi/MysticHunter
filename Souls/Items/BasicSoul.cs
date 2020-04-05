using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Items
{
	/// <summary>
	/// The basic class for a soul item.
	/// </summary>
	public class BasicSoul : ModItem
	{
		public short soulNPC = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul");
		}
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
					Main.NewText("You collected the " + s.SoulNPCName() + " soul.", c);
				}
			}
			else
			{
				// Debug message for when a soul with the given `soulDataID` cannot be found.
				Main.NewText("Soul with ID '" + soulNPC + "' cannot be found.");
			}

			return (false);
		}

		public override bool ItemSpace(Player player) => true;

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			SoulType type = MysticHunter.Instance.SoulDict[soulNPC].soulType;

			Texture2D tex = Main.itemTexture[item.type];
			Rectangle rect = new Rectangle(0, 0, 16, 16);
			Vector2 origin = new Vector2(tex.Width * .5f, (tex.Height / 3) * .5f);

			if (type == SoulType.Blue)
				rect.Y += 16;
			if (type == SoulType.Yellow)
				rect.Y += 32;

			spriteBatch.Draw(tex, item.position - Main.screenPosition + origin, rect, lightColor, 0, origin, 1, SpriteEffects.None, 0);

			return (false);
		}
	}
}
