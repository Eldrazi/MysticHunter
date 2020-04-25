using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	public static class UIUtilities
	{
		public static void DrawPanelBorders(SpriteBatch spriteBatch, Texture2D tex, Rectangle rect, int cornerSize, int midsectionSize, bool fillPanel = false)
		{
			DrawPanelBorders(spriteBatch, tex, rect.X, rect.Y, rect.Width, rect.Height, cornerSize, midsectionSize, fillPanel);
		}

		public static void DrawPanelBorders(SpriteBatch spriteBatch, Texture2D tex, int x, int y, int width, int height, int cornerSize, int midsectionSize, bool fillPanel = false)
		{
			Rectangle destinationRect, sourceRect;

			// Draw upper left corner.
			sourceRect = new Rectangle(0, 0, cornerSize, cornerSize);
			destinationRect = new Rectangle(x, y, cornerSize, cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw upper line.
			sourceRect = new Rectangle(cornerSize, 0, midsectionSize, cornerSize);
			destinationRect = new Rectangle(x + cornerSize, y, width - 2 * cornerSize, cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw upper right corner.
			sourceRect = new Rectangle(cornerSize + midsectionSize, 0, cornerSize, cornerSize);
			destinationRect = new Rectangle(x + width - cornerSize, y, cornerSize, cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw left line.
			sourceRect = new Rectangle(0, cornerSize, cornerSize, midsectionSize);
			destinationRect = new Rectangle(x, y + cornerSize, cornerSize, height - 2 * cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw right line.
			sourceRect = new Rectangle(cornerSize + midsectionSize, cornerSize, cornerSize, midsectionSize);
			destinationRect = new Rectangle(x + width - cornerSize, y + cornerSize, cornerSize, height - 2 * cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw lower left corner.
			sourceRect = new Rectangle(0, cornerSize + midsectionSize, cornerSize, cornerSize);
			destinationRect = new Rectangle(x, y + height - cornerSize, cornerSize, cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw lower line.
			sourceRect = new Rectangle(cornerSize, cornerSize + midsectionSize, midsectionSize, cornerSize);
			destinationRect = new Rectangle(x + cornerSize, y + height - cornerSize, width - 2 * cornerSize, cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			// Draw lower right corner.
			sourceRect = new Rectangle(cornerSize + midsectionSize, cornerSize + midsectionSize, cornerSize, cornerSize);
			destinationRect = new Rectangle(x + width - cornerSize, y + height - cornerSize, cornerSize, cornerSize);
			spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);

			if (fillPanel)
			{
				sourceRect = new Rectangle(cornerSize, cornerSize, midsectionSize, midsectionSize);
				destinationRect = new Rectangle(x + cornerSize, y + cornerSize, width - 2 * cornerSize, height - 2 * cornerSize);
				spriteBatch.Draw(tex, destinationRect, sourceRect, Color.White);
			}
		}
	}
}
