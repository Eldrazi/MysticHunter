using Terraria;
using Terraria.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	internal class SoulIndexUITitleBar : UIElement
	{
		public string content;
		private Vector2 contentDimensions;

		private readonly Texture2D titleBarTexture;

		public SoulIndexUITitleBar(Texture2D titleBarTexture, string content = "Empty")
		{
			this.SetContent(content);
			this.titleBarTexture = titleBarTexture;
		}

		public void SetContent(string content)
		{
			this.content = content;
			this.contentDimensions = (Vector2)(Main.fontMouseText?.MeasureString(content) ?? Vector2.Zero);

			// Give the X dimension extra padding.
			this.contentDimensions.X += 16;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle titleBarDestination = this.GetDimensions().ToRectangle();
			titleBarDestination.Width = 16;
			titleBarDestination.Height = 30;

			Rectangle titleBarSource = new Rectangle(0, 0, 16, 30);
			spriteBatch.Draw(titleBarTexture, titleBarDestination, titleBarSource, Color.White);

			titleBarDestination.X += titleBarDestination.Width;
			titleBarDestination.Width = (int)this.contentDimensions.X;
			titleBarSource = new Rectangle(16, 0, 4, 30);
			spriteBatch.Draw(titleBarTexture, titleBarDestination, titleBarSource, Color.White);

			titleBarDestination.X += titleBarDestination.Width;
			titleBarDestination.Width = 16;
			titleBarSource = new Rectangle(22, 0, 16, 30);
			spriteBatch.Draw(titleBarTexture, titleBarDestination, titleBarSource, Color.White);

			titleBarDestination.X -= (int)this.contentDimensions.X - 8;
			titleBarDestination.Y += 6;
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, this.content, titleBarDestination.X, titleBarDestination.Y, Color.White, Color.Black, Vector2.Zero, 1);
		}
	}
}
