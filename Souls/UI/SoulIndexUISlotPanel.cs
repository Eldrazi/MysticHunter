﻿using Terraria;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using Terraria.UI;

namespace MysticHunter.Souls.UI
{
	internal class SoulIndexUISlotPanel : GenericUIPanel
	{
		public SoulIndexUISoulSlot[] soulSlots;

		private SoulIndexPanel parent;

		public SoulIndexUISlotPanel(SoulIndexPanel parent, Texture2D panelTexture, Vector2 panelDimensions) : base(panelTexture, panelDimensions)
		{
			this.parent = parent;
		}

		public override void OnInitialize()
		{
			this.SetPadding(0);

			this.Top.Pixels = 18;
			this.Left.Pixels = 0;

			this.Width.Pixels = this.Parent.Width.Pixels / 2 - 8;
			this.Height.Pixels = this.Parent.Height.Pixels - 118 - this.Top.Pixels;

			soulSlots = new SoulIndexUISoulSlot[3];
			for (int i = 0; i < soulSlots.Length; ++i)
			{
				soulSlots[i] = new SoulIndexUISoulSlot
				{
					soulType = (SoulType)i
				};

				soulSlots[i].Top.Pixels = 20 + 32 * i;
				soulSlots[i].Left.Pixels = 10;

				soulSlots[i].Width.Pixels = this.Width.Pixels - 28;
				soulSlots[i].Height.Pixels = 18;

				soulSlots[i].OnClick += LeftClick;

				this.Append(soulSlots[i]);
			}
		}

		public override void Update(GameTime gameTime)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();
			soulSlots[0].SetSoulReference(sp.RedSoul, false);
			soulSlots[1].SetSoulReference(sp.BlueSoul, false);
			soulSlots[2].SetSoulReference(sp.YellowSoul, false);
		}

		private void LeftClick(UIMouseEvent evt, UIElement e)
		{
			if (parent.soulListPanel != null)
			{
				if (!(e is SoulIndexUISoulSlot slot))
					return;

				// If the new filter is the same as the filter that's already there, just ignore.
				if (parent.soulListPanel.soulList.filter == slot.soulType)
					return;

				// Set the correct filter.
				parent.soulListPanel.soulList.filter = slot.soulType;
				SoulManager.ReloadSoulIndexUI();
			}
		}
	}
}