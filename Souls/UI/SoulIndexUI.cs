using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Items;

namespace MysticHunter.Souls.UI
{
	public class SoulIndexUI : UIState
	{
		public static bool visible
		{
			get { return Main.playerInventory && Main.LocalPlayer.inventory[MysticHunter.Instance.selectedItem].type == ItemType<SoulIndex>(); }
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
		internal Texture2D panelTexture;
		internal Rectangle drawRectangle => new Rectangle((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels);

		internal SoulItemBox[] soulItemBoxes;

        public Vector2[] soulBoxPositions = new Vector2[3]
        {
            new Vector2(34, 12),
            new Vector2(98, 12),
            new Vector2(34, 80),
        };

		public override void OnInitialize()
		{
			panelTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_Panel");

			this.SetPadding(0);
			this.Left.Pixels = 160;
			this.Top.Pixels = 260;
			this.Width.Pixels = panelTexture.Width;
			this.Height.Pixels = panelTexture.Height;

			soulItemBoxes = new SoulItemBox[3];
			for (int i = 0; i < soulItemBoxes.Length; ++i)
			{
				soulItemBoxes[i] = new SoulItemBox
				{
					soulSlot = (SoulType)i
				};
				soulItemBoxes[i].Top.Pixels = soulBoxPositions[i].Y;
				soulItemBoxes[i].Left.Pixels = soulBoxPositions[i].X;
				soulItemBoxes[i].SetCondition();

				this.Append(soulItemBoxes[i]);
			}
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
			spriteBatch.Draw(panelTexture, drawRectangle, Color.White);
		}
	}

	/// <summary>
	/// A conditional check to see if an item is valid for a given slot.
	/// </summary>
	/// <param name="item">The item that needs to be checked.</param>
	/// <returns>Returns whether or not the item </returns>
	internal delegate bool Condition(Item item);

	internal class SoulItemBox : UIPanel
	{
		Texture2D boxTexture;

		public Condition condition;

		public SoulType soulSlot;

		public Rectangle drawRectangle => new Rectangle((int)(Parent.Left.Pixels + Left.Pixels), (int)(Parent.Top.Pixels + Top.Pixels), (int)Width.Pixels, (int)Height.Pixels);

		public override void OnInitialize()
		{
			boxTexture = GetTexture("MysticHunter/Souls/UI/SoulIndex_ItemBox");

			Width.Pixels = boxTexture.Width;
			Height.Pixels = boxTexture.Height;

			this.OnClick += ItemBoxClick;
		}

		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;
		}

		// This function needs to be doubly checked.
		// Needs to incorporate a check.
		public void SetCondition()
		{
			this.condition = delegate (Item item)
			{
				Mod mod = ModLoader.GetMod("MysticHunter");
				if (item.modItem != null)
				{
					Main.NewText(item.modItem is BasicSoul);
					return (item.modItem is BasicSoul soul && soul.soulType == this.soulSlot);
				}
				return (false);
			};
		}

		// Clicking functionality.
		private void ItemBoxClick(UIMouseEvent evt, UIElement e)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.souls[(int)soulSlot] != null && !sp.souls[(int)soulSlot].IsAir)
			{
				if (Main.mouseItem.IsAir)
				{
					Main.PlaySound(SoundID.Grab);
					Main.mouseItem = sp.souls[(int)soulSlot].Clone();

					sp.souls[(int)soulSlot].TurnToAir();
				}
				else if (condition == null || (condition != null && condition(Main.mouseItem)))
				{
					Main.PlaySound(SoundID.Grab);

					Item tmpBoxItem = sp.souls[(int)soulSlot].Clone();

					sp.souls[(int)soulSlot] = Main.mouseItem.Clone();
					Main.mouseItem = tmpBoxItem;
				}
			}
			else if (!Main.mouseItem.IsAir)
			{
				if (condition == null || (condition != null && condition(Main.mouseItem)))
				{
					Main.PlaySound(SoundID.Grab);
					sp.souls[(int)soulSlot] = Main.mouseItem.Clone();
					Main.mouseItem.TurnToAir();
				}
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			spriteBatch.Draw(boxTexture, drawRectangle, Color.White);

			// Check to see if the Soul item in the SoulPlayer is set.
			// If not, there's no reason to try and draw it.
			Item drawTarget = sp.souls[(int)soulSlot];
			if (drawTarget == null || drawTarget.IsAir) return;

			// Vanilla inventory item drawing code.
			// Fairly straightforward, no real need to try and comprehend this.
			// We probably won't touch this code too much!.
			Color itemColor = drawTarget.GetAlpha(Color.White);
			Texture2D itemTexture = Main.itemTexture[drawTarget.type];
			CalculatedStyle innerDimensions = base.GetDimensions();

			if (base.IsMouseHovering)
			{
				Main.hoverItemName = drawTarget.Name;
				Main.HoverItem = drawTarget.Clone();
			}

			Rectangle frame = Main.itemAnimations[drawTarget.type] != null
						? Main.itemAnimations[drawTarget.type].GetFrame(itemTexture)
						: itemTexture.Frame(1, 1, 0, 0);

			float drawScale = 1f;
			if (frame.Width > innerDimensions.Width || frame.Height > innerDimensions.Width)
			{
				drawScale = innerDimensions.Width;
				if (frame.Width > frame.Height)
					drawScale /= frame.Width;
				else
					drawScale /= frame.Height;
			}

			Color tmpcolor = Color.White;

			ItemSlot.GetItemLight(ref tmpcolor, ref drawScale, drawTarget.type);

			Vector2 drawPosition = new Vector2(innerDimensions.X, innerDimensions.Y);

			drawPosition.X += innerDimensions.Width * 1f / 2f - frame.Width * drawScale / 2f;
			drawPosition.Y += innerDimensions.Height * 1f / 2f - frame.Height * drawScale / 2f;

			spriteBatch.Draw(itemTexture, drawPosition, new Rectangle?(frame), itemColor, 0f,
				Vector2.Zero, drawScale, SpriteEffects.None, 0f);

			if (drawTarget.color != default)
			{
				spriteBatch.Draw(itemTexture, drawPosition, new Rectangle?(frame), itemColor, 0f,
					Vector2.Zero, drawScale, SpriteEffects.None, 0f);
			}
		}
	}
}
