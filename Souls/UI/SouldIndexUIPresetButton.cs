#region Using directives

using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace MysticHunter.Souls.UI
{
	public class SouldIndexUIPresetButton : UIElement
	{
		public int presetIndex;

		private Texture2D presetButtonTexture;

		private bool IsSelected
		{
			get
			{
				SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();
				return (sp.activeSoulConfig == this.presetIndex);
			}
		}

		public SouldIndexUIPresetButton(int presetIndex)
		{
			this.presetIndex = presetIndex;

			this.OnClick += PresetButtonLeftClick;

			presetButtonTexture = ModContent.GetTexture("MysticHunter/Souls/UI/SoulIndex_Tab" + (presetIndex + 1)).Value;

			this.Left.Pixels = 0;
			this.Top.Pixels = 28 + 36 * presetIndex;

			this.Width.Pixels = 46;
			this.Height.Pixels = 32;
		}

		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;
		}

		private void PresetButtonLeftClick(UIMouseEvent evt, UIElement e)
		{
			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (this.presetIndex >= sp.activeSouls.GetLength(0))
				return;

			sp.activeSoulConfig = this.presetIndex;
			sp.UpdateActiveSoulData();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Rectangle drawRect = this.GetDimensions().ToRectangle();
			Color drawColor = Color.White * (this.IsSelected ? 1 : this.IsMouseHovering ? .75f : .5f);
			spriteBatch.Draw(this.presetButtonTexture, drawRect, drawColor);
		}
	}
}
