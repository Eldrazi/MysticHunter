using System.Linq;

using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.UI
{
	internal class SoulIndexUIListPanel : UIElement
	{
		public SoulIndexUIList soulList;

		public SoulItemBox[] soulItemBoxReferences;

		public override void OnInitialize()
		{
			this.SetPadding(0);

			this.Top.Pixels = 8;
			this.Left.Pixels = 8;

			soulList = new SoulIndexUIList();
			this.Append(soulList);

			SoulIndexUIScrollbar listScrollbar = new SoulIndexUIScrollbar();
			listScrollbar.SetView(100f, 1000f);
			listScrollbar.Top.Pixels = 8;
			listScrollbar.Left.Pixels = this.Width.Pixels - 18;
			listScrollbar.Height.Pixels = this.Height.Pixels - 16;
			this.Append(listScrollbar);
			soulList.SetScrollbar(listScrollbar);
		}
	}

	internal class SoulIndexUIList : GenericUIList
	{
		public SoulType filter;

		public override void OnInitialize()
		{
			this.Top.Pixels = 8;
			this.Left.Pixels = 8;
			this.Width.Pixels = Parent.Width.Pixels - 22;
			this.Height.Pixels = Parent.Height.Pixels - 16;

			// Auto-set soul filter to red.
			filter = SoulType.Red;
		}

		public void ReloadList()
		{
			this.Clear();

			MysticHunter.Instance.SoulDict.Values.Where(v => v.acquired == true && v.soulType == filter)
				.ToList()
				.ForEach(
				v => this.Add(new SoulIndexUIListItem(v))
			);
		}
	}

	internal class SoulIndexUIScrollbar : GenericUIScrollbar
	{
		public SoulIndexUIScrollbar() : base()
		{
			thumb = GetTexture("MysticHunter/Souls/UI/SoulIndex_ScrollThumb");
			background = GetTexture("MysticHunter/Souls/UI/SoulIndex_ScrollBackground");

			thumbStumpSize = 10;
		}
	}

	internal class SoulIndexUIListItem : UIPanel
	{
		private BaseSoul soulReference;

		private Texture2D panelTexture;

		private Texture2D[] soulTextures;
		private Texture2D soulSlotTexture;

		public SoulIndexUIListItem(BaseSoul soulReference)
		{
			this.soulReference = soulReference;

			panelTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_GenericPanel");

			soulTextures = new Texture2D[3];
			soulTextures[0] = GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulRed");
			soulTextures[1] = GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulBlue");
			soulTextures[2] = GetTexture("MysticHunter/Souls/UI/SoulIndex_SoulYellow");

			soulSlotTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_ItemPanel");

			this.OnDoubleClick += SetSoulSlot;

			this.SetPadding(5);
			this.Width = StyleDimension.Fill;
			this.Height.Pixels = 80;
		}

		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;
		}

		private void SetSoulSlot(UIMouseEvent evt, UIElement e)
		{
			int soulIndex = (int)this.soulReference.soulType;

			Main.LocalPlayer.GetModPlayer<SoulPlayer>().souls[soulIndex] = soulReference;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle hitbox = GetInnerDimensions().ToRectangle();

			// Draw the borders for this list item.
			UIUtilities.DrawPanelBorders(spriteBatch, panelTexture, hitbox, 14, 2, true);

			// Draw the Soul Slot.
			Rectangle soulSlotRect = new Rectangle(hitbox.X + 8, hitbox.Y + 8, soulSlotTexture.Width, soulSlotTexture.Height);
			spriteBatch.Draw(soulSlotTexture, soulSlotRect, Color.White);

			// Draw the corresponding soul inside the Soul Slot.
			soulSlotRect.X += (soulSlotRect.Width / 2) - soulTextures[(int)soulReference.soulType].Width / 2;
			soulSlotRect.Y += (soulSlotRect.Height / 2) - soulTextures[(int)soulReference.soulType].Height / 2;
			soulSlotRect.Width = soulTextures[(int)soulReference.soulType].Width;
			soulSlotRect.Height = soulTextures[(int)soulReference.soulType].Height;
			spriteBatch.Draw(soulTextures[(int)soulReference.soulType], soulSlotRect, Color.White);

			// Draw the name of the soul/associated NPC next to the Soul Slot.
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, soulReference.SoulNPCName() + " soul", hitbox.X + 40, hitbox.Y + 12, Color.White, Color.Black, Vector2.Zero, .8f);

			// Draw the description of the soul under the Soul Slot.
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, soulReference.soulDescription, hitbox.X + 12, hitbox.Y + 38, Color.White, Color.Black, Vector2.Zero, .8f);
		}
	}
}
