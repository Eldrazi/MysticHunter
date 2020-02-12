using Terraria.UI;

using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.UI
{
	/// <summary>
	/// A visual 'Soul Slot' that can display a soul inside of it.
	/// Consists of two images: a 'slot' image (border) and a soul image.
	/// </summary>
	public class SoulIndexUISoulSlot : UIElement
	{
		private Texture2D slotTexture;
		private Texture2D[] soulTextures;

		public ISoul soulTarget = null;

		public SoulIndexUISoulSlot(ISoul soulTarget = null)
		{
			slotTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_ItemPanel");

			soulTextures = new Texture2D[3]
			{
				GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulRed"),
				GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulBlue"),
				GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulYellow")
			};

			this.Width.Pixels = slotTexture.Width;
			this.Height.Pixels = slotTexture.Height;

			this.soulTarget = soulTarget;
		}

		public void SetSoulTarget(ISoul soulTarget)
		{
			this.soulTarget = soulTarget;
		}

		// Ignore the DrawSelf functionality.
		// This class is supposed to be drawn manually by calling `DrawSlot`.
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle drawRect = this.GetDimensions().ToRectangle();

			spriteBatch.Draw(slotTexture, drawRect, Color.White);

			if (soulTarget != null && soulTarget.soulNPC != 0)
			{
				drawRect = new Rectangle(drawRect.X + drawRect.Width / 2 - soulTextures[0].Width / 2,
					drawRect.Y + drawRect.Height / 2 - soulTextures[0].Height / 2, soulTextures[0].Width, soulTextures[0].Height);
				spriteBatch.Draw(soulTextures[(int)soulTarget.soulType], drawRect, Color.White);
			}
		}
	}
}
