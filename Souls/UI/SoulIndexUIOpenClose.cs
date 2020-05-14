using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	public class SoulIndexUIOpenClose : UIState
	{
		public static bool visible
			=> Main.playerInventory;

		internal SoulIndexUIOpenClosePanel openClosePanel;

		public override void OnInitialize()
		{
			openClosePanel = new SoulIndexUIOpenClosePanel();
			openClosePanel.Initialize();

			this.Append(openClosePanel);
		}
	}

	internal class SoulIndexUIOpenClosePanel : UIPanel
	{
		private Texture2D texture;

		public override void OnInitialize()
		{
			texture = GetTexture("MysticHunter/Souls/UI/SoulIndex_OpenCloseButton");

			this.SetPadding(0);
			this.Top.Pixels = 28;
			this.Left.Pixels = 505;

			this.Width.Pixels = this.Height.Pixels = 52;

			this.OnClick += OpenCloseSoulIndexUI;
		}

		private void OpenCloseSoulIndexUI(UIMouseEvent evt, UIElement e)
			=> SoulIndexUI.visible = !SoulIndexUI.visible;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (this.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;

			Rectangle drawRect = this.GetDimensions().ToRectangle();
			Color color = Color.White * (this.IsMouseHovering ? 1 : .75f);
			spriteBatch.Draw(texture, drawRect, color);
		}
	}
}
