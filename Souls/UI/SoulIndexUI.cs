using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Items;
using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	public class SoulIndexUI : UIState
	{
		public static bool visible
		{
			get { return Main.playerInventory && Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].type == ItemType<SoulIndex>(); }
		}

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
		internal Texture2D skullTexture;
		internal Texture2D panelTexture;

		internal Rectangle soulSlotRect;
		internal const int soulSlotPanelWidth = 152, soulSlotPanelHeight = 50;

		internal SoulItemBox[] soulItemBoxes;

		public SoulIndexUIListPanel soulListPanel;

		public Vector2[] soulBoxPositions;

		public override void OnInitialize()
		{
			skullTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_SkullFlair");
			panelTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_GenericPanel");

			this.SetPadding(0);
			this.Left.Pixels = 82;
			this.Top.Pixels = 260;
			this.Width.Pixels = 350;
			this.Height.Pixels = 200;

			// Set the rectangle/position of the panel in which the soul slots are displayed.
			soulSlotRect = new Rectangle((int)((Width.Pixels / 2) - (soulSlotPanelWidth / 2)), (int)(Height.Pixels - 14), soulSlotPanelWidth, soulSlotPanelHeight);

			// Dynamically add the soul slots/boxes to the soulSlotRect panel.
			// Automatically center every slot/box and add relevant padding depending on the dimensions of the soulSlotRect.
			soulItemBoxes = new SoulItemBox[3];
			for (int i = 0; i < soulItemBoxes.Length; ++i)
			{
				soulItemBoxes[i] = new SoulItemBox
				{
					soulSlot = (SoulType)i
				};

				// Hardcoded dimension values of the slot textures (14).
				soulItemBoxes[i].Top.Pixels = soulSlotRect.Y + (soulSlotPanelHeight / 2) - 12;
				soulItemBoxes[i].Left.Pixels = soulSlotRect.X + 14 + (soulSlotPanelWidth / 3) * i;

				this.Append(soulItemBoxes[i]);
			}

			soulListPanel = new SoulIndexUIListPanel();
			soulListPanel.Width.Pixels = this.Width.Pixels - 16;
			soulListPanel.Height.Pixels = this.Height.Pixels - 16;
			this.Append(soulListPanel);

			// Add the soulSlotPanelHeight to the height of the UI element.
			// This is so that the soul slot panel that hangs under this panel is actually interactable.
			this.Height.Pixels += soulSlotPanelHeight;
			this.Recalculate();
		}

		public override void Update(GameTime gameTime)
		{
			Top.Pixels = 260;
			if (Main.LocalPlayer.chest != -1 || Main.npcShop != 0)
				Top.Pixels += 170;

			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			// Substract the soulSlotPanelHeight of this panels' height to accomodate for the added space in OnInitialize.
			Rectangle panelRectangle = this.GetDimensions().ToRectangle();
			panelRectangle.Height -= soulSlotPanelHeight;

			// Draw the main panel.
			UIUtilities.DrawPanelBorders(spriteBatch, panelTexture, panelRectangle, 14, 2, true);

			// Draw the skull flair on the main panel.
			Rectangle skullRectangle = new Rectangle((int)(Left.Pixels + Width.Pixels / 2) - (skullTexture.Width / 2), (int)(Top.Pixels - 4), skullTexture.Width, skullTexture.Height);
			spriteBatch.Draw(skullTexture, skullRectangle, Color.White);

			// Draw the souls panel.
			Rectangle soulSlotsDrawRect = new Rectangle(soulSlotRect.X + (int)Left.Pixels, soulSlotRect.Y + (int)Top.Pixels, soulSlotRect.Width, soulSlotRect.Height);
			UIUtilities.DrawPanelBorders(spriteBatch, panelTexture, soulSlotsDrawRect, 14, 2, true);
		}
	}

	internal class SoulItemBox : UIPanel
	{
		Texture2D itemPanel;
		Texture2D[] soulTextures;

		public SoulType soulSlot;

		public Rectangle drawRectangle => new Rectangle((int)(Parent.Left.Pixels + Left.Pixels), (int)(Parent.Top.Pixels + Top.Pixels), (int)Width.Pixels, (int)Height.Pixels);

		public override void OnInitialize()
		{
			itemPanel = GetTexture("MysticHunter/Souls/UI/SoulIndex_ItemPanel");

			Width.Pixels = itemPanel.Width;
			Height.Pixels = itemPanel.Height;

			soulTextures = new Texture2D[3];
			soulTextures[0] = GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulRed");
			soulTextures[1] = GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulBlue");
			soulTextures[2] = GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulYellow");

			this.OnClick += LeftClick;
			this.OnRightClick += RightClick;
		}

		/// <summary>
		/// Makes sure mouse clicks are registered as UI clicks and not as game-functionality related clicks.
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;
		}

		/// <summary>
		/// Add a stack to the click soul.
		/// </summary>
		private void LeftClick(UIMouseEvent evt, UIElement e)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.souls[(int)soulSlot] != null && sp.soulsStack[(int)soulSlot] < 9)
				sp.soulsStack[(int)soulSlot]++;
		}
		/// <summary>
		/// Removes a soul from its slot if it's right clicked.
		/// </summary>
		private void RightClick(UIMouseEvent evt, UIElement e)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();
			sp.souls[(int)soulSlot] = null;
			sp.soulsStack[(int)soulSlot] = 1;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			spriteBatch.Draw(itemPanel, drawRectangle, Color.White);

			ISoul soulReference = sp.souls[(int)soulSlot];

			if (base.IsMouseHovering)
			{
				if (soulReference == null || soulReference.soulNPC == 0)
					Main.hoverItemName = "No Soul";
				else
					Main.hoverItemName = soulReference.soulName + " soul";
			}

			// Check to see if the Soul item in the SoulPlayer is set.
			if (soulReference != null)
			{
				Rectangle soulRect = new Rectangle(drawRectangle.X + drawRectangle.Width / 2 - soulTextures[0].Width / 2,
					drawRectangle.Y + drawRectangle.Height / 2 - soulTextures[0].Height / 2, soulTextures[0].Width, soulTextures[0].Height);
				spriteBatch.Draw(soulTextures[(int)soulReference.soulType], soulRect, Color.White);

				Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, sp.soulsStack[(int)soulSlot].ToString(), 
					drawRectangle.X + 6, drawRectangle.Y + drawRectangle.Height - 12, Color.White, Color.Black, Vector2.Zero, .6f);
			}
		}
	}
}
