﻿#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.Audio;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.UI
{
	internal class SoulIndexUIListPanel : GenericUIPanel
	{
		internal const int height = 170;
		private readonly SoulIndexPanel soulPanel;

		public SoulIndexUIList soulList;

		private SoulIndexUITitleBar titleBar;

		public SoulIndexUIListPanel(SoulIndexPanel soulPanel, Texture2D panelTexture, Vector2 panelDimensions) : base(panelTexture, panelDimensions)
		{
			this.soulPanel = soulPanel;
		}

		public override void OnInitialize()
		{
			this.SetPadding(0);

			this.Top.Pixels = this.Parent.Height.Pixels - height;

			this.Height.Pixels = height;
			this.Width.Pixels = this.Parent.Width.Pixels;

			soulList = new SoulIndexUIList(soulPanel);
			this.Append(soulList);

			titleBar = new SoulIndexUITitleBar(ModContent.GetTexture("MysticHunter/Souls/UI/SoulIndex_TitleBar").Value, "");
			titleBar.Top.Pixels = -12;
			titleBar.Left.Pixels = 60;
			this.Append(titleBar);

			SoulIndexUIScrollbar listScrollbar = new SoulIndexUIScrollbar();
			listScrollbar.SetView(100f, 1000f);
			listScrollbar.Top.Pixels = 8;
			listScrollbar.Left.Pixels = this.Width.Pixels - 18;
			listScrollbar.Height.Pixels = this.Height.Pixels - 16;
			this.Append(listScrollbar);
			soulList.SetScrollbar(listScrollbar);
		}

		/// <summary>
		/// Updates the Title Bar content/string depending on the <see cref="SoulIndexUIList"/> filter.
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			string requiredString = this.soulList.filter.ToString() + " Souls";
			if (requiredString != titleBar.content)
				titleBar.SetContent(requiredString);

			base.Update(gameTime);
		}
	}

	internal class SoulIndexUIList : GenericUIList
	{
		public SoulType filter = SoulType.Red;

		private readonly SoulIndexPanel soulPanel;

		public SoulIndexUIList(SoulIndexPanel soulPanel)
		{
			this.soulPanel = soulPanel;
		}

		public override void OnInitialize()
		{
			this.ListWidth = 2;
			this.ListPaddingX = 12;

			this.Top.Pixels = 18;
			this.Left.Pixels = 16;
			this.Width.Pixels = Parent.Width.Pixels - 18;
			this.Height.Pixels = Parent.Height.Pixels - 34;
		}

		public void ReloadList()
		{
			this.Clear();

			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();
			foreach (short key in sp.UnlockedSouls.Keys)
			{
				if (!MysticHunter.Instance.SoulDict.TryGetValue(key, out BaseSoul value) || value.soulType != filter)
					continue;

				SoulIndexUISoulSlot newSoulSlot = new SoulIndexUISoulSlot(value);

				newSoulSlot.Width.Pixels = this.Width.Pixels / 2 - 12;

				newSoulSlot.OnClick += SetSoulSlot;
				newSoulSlot.OnMouseOver += SetDescriptionPanelContent;
				newSoulSlot.OnMouseOut += ResetDescriptionPanelContent;
				this.Add(newSoulSlot);
			}
		}

		private void SetSoulSlot(UIMouseEvent evt, UIElement e)
		{
			if (!(e is SoulIndexUISoulSlot slot))
				return;

			int soulIndex = (int)slot.soulReference.soulType;
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			// TODO: Eldrazi - Maybe change the sound?
			SoundEngine.PlaySound(SoundID.Item37);
			sp.activeSouls[sp.activeSoulConfig, soulIndex].soulNPC = slot.soulReference.soulNPC;
			sp.UpdateActiveSoulData();
		}

		private void SetDescriptionPanelContent(UIMouseEvent evt, UIElement e)
		{
			if (!(e is SoulIndexUISoulSlot slot))
				return;

			soulPanel.soulDescriptionPanel.SetSoulReference(slot.soulReference);
		}
		private void ResetDescriptionPanelContent(UIMouseEvent evt, UIElement e)
			=> soulPanel.soulDescriptionPanel.SetSoulReference(null);
	}

	internal class SoulIndexUIScrollbar : GenericUIScrollbar
	{
		public SoulIndexUIScrollbar() : base()
		{
			thumbStumpSize = 10;

			this.smoothScroll = true;
		}
	}
}
