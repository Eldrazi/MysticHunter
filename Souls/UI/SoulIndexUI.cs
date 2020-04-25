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

		public override void OnInitialize()
		{
			panelTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_GenericPanel");

			this.SetPadding(0);
			this.Left.Pixels = 82;
			this.Top.Pixels = 260;
			this.Width.Pixels = 400;
			this.Height.Pixels = 260;

			// Add the SoulIndexUIListPanel, which controls the scrolling list of acquired souls.
			soulListPanel = new SoulIndexUIListPanel(this, panelTexture, new Vector2(24, 8));
			this.Append(soulListPanel);

			soulSlotPanel = new SoulIndexUISlotPanel(this, panelTexture, new Vector2(24, 8));
			this.Append(soulSlotPanel);

			soulDescriptionPanel = new SoulIndexUIDescriptionPanel(panelTexture, new Vector2(24, 8));
			this.Append(soulDescriptionPanel);
		}

		public override void Update(GameTime gameTime)
		{
			Top.Pixels = Main.screenHeight / 2 - this.Height.Pixels / 2;
			Left.Pixels = Main.screenWidth / 2 - this.Width.Pixels / 2;

			if (SoulIndexUI.visible && !Main.playerInventory)
				SoulIndexUI.visible = false;

			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (this.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;
		}
	}
}
