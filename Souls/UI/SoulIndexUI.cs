using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	public class SoulIndexUI : UIState
	{
		public static bool visible = false;

		internal SoulIndexPanel soulIndexPanel;

		public override void OnInitialize()
		{
			soulIndexPanel = new SoulIndexPanel();
			soulIndexPanel.Initialize();

			this.Append(soulIndexPanel);
		}
	}

	internal class SoulIndexPanel : UIPanel
	{
		internal Texture2D panelTexture;

		public SoulIndexUIListPanel soulListPanel;

		public SoulIndexUISlotPanel soulSlotPanel;

		public SoulIndexUIDescriptionPanel soulDescriptionPanel;

		public SoulIndexUICloseButton closeButton;

		private bool dragging = false;
		private Vector2 dragPos = Vector2.Zero;
		private Vector2 relativeDragpos = Vector2.Zero;

		public override void OnInitialize()
		{
			panelTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_GenericPanel");

			this.SetPadding(0);
			this.Width.Pixels = 400;
			this.Height.Pixels = 330;

			// Add the SoulIndexUIListPanel, which controls the scrolling list of acquired souls.
			soulListPanel = new SoulIndexUIListPanel(this, panelTexture, new Vector2(24, 8));
			this.Append(soulListPanel);

			soulSlotPanel = new SoulIndexUISlotPanel(this, panelTexture, new Vector2(24, 8));
			this.Append(soulSlotPanel);

			closeButton = new SoulIndexUICloseButton(GetTexture("MysticHunter/Souls/UI/SoulIndex_CloseButton"));
			this.Append(closeButton);

			soulDescriptionPanel = new SoulIndexUIDescriptionPanel(panelTexture, new Vector2(24, 8));
			this.Append(soulDescriptionPanel);

			Top.Pixels = Main.screenHeight / 2 - this.Height.Pixels / 2;
			Left.Pixels = Main.screenWidth / 2 - this.Width.Pixels / 2;

			this.OnMouseUp += EndDrag;
			this.OnMouseDown += StartDrag;
		}

		private void StartDrag(UIMouseEvent evt, UIElement e)
		{
			dragging = true;
			dragPos = Main.MouseScreen;
			relativeDragpos = new Vector2(dragPos.X - this.Left.Pixels, dragPos.Y - this.Top.Pixels);
		}
		private void EndDrag(UIMouseEvent evt, UIElement e) => dragging = false;

		public override void Update(GameTime gameTime)
		{
			if (!Main.playerInventory)
				SoulIndexUI.visible = false;
			else
			{
				if (dragging && Main.MouseScreen != this.dragPos)
				{
					this.Top.Pixels = Main.MouseScreen.Y - relativeDragpos.Y;
					this.Left.Pixels = Main.MouseScreen.X - relativeDragpos.X;
					this.dragPos = Main.MouseScreen;
				}

				this.Top.Pixels = MathHelper.Clamp(this.Top.Pixels, 0, Main.screenHeight - this.Height.Pixels);
				this.Left.Pixels = MathHelper.Clamp(this.Left.Pixels, 0, Main.screenWidth - this.Width.Pixels);
			}

			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (this.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;
		}

		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			soulListPanel.Draw(spriteBatch);
			soulSlotPanel.Draw(spriteBatch);
			soulDescriptionPanel.Draw(spriteBatch);
			closeButton.Draw(spriteBatch);
		}
	}
}
