#region Using directives

using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace MysticHunter.Souls.UI
{
	public class SoulIndexUICloseButton : UIPanel
	{
		private Texture2D texture;

		public SoulIndexUICloseButton(Texture2D texture)
		{
			this.texture = texture;
		}

		public override void OnInitialize()
		{
			this.SetPadding(0);
			this.Top.Pixels = -4;
			this.Left.Pixels = this.Parent.Width.Pixels - 16;

			this.Width.Pixels = this.Height.Pixels = 20;

			this.OnClick += CloseSoulIndexUI;
		}

		private void CloseSoulIndexUI(UIMouseEvent evt, UIElement e)
			=> SoulIndexUI.visible = false;

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
