using Terraria;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.UI
{
	public class SoulIndexUISoulSlot : UIElement
	{
		public BaseSoul soulReference;
		public SoulType soulType;

		private readonly Texture2D soulTexture;

		public SoulIndexUISoulSlot(BaseSoul soulReference = null)
		{
			this.soulReference = soulReference;
			this.soulType = this.soulReference?.soulType ?? SoulType.Red;

			this.soulTexture = GetTexture("MysticHunter/Souls/Items/BasicSoulItem");
		}

		public void SetSoulReference(BaseSoul soul, bool overrideType = true)
		{
			this.soulReference = soul;
			if (overrideType)
				this.soulType = this.soulReference?.soulType ?? SoulType.Red;
		}

		public override void Update(GameTime gameTime)
		{
			if (IsMouseHovering)
				Main.LocalPlayer.mouseInterface = true;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			// Draw corresponding soul texture.
			Vector2 drawPos = this.GetDimensions().Position();
			Rectangle sourceRectangle = new Rectangle(0, 16 * (int)this.soulType, 16, 16);

			spriteBatch.Draw(soulTexture, drawPos, sourceRectangle, Color.White);

			if (soulReference == null)
				return;

			string nameString = this.soulReference.SoulNPCName();

			// Draw the name of the referenced soul.
			if (nameString.Length >= 15)
				nameString = nameString.Substring(0, 15) + "...";

			drawPos.X += 20;
			Color color = !this.IsMouseHovering ? Color.White : new Color(Main.mouseTextColor, (int)(Main.mouseTextColor / 1.1F), Main.mouseTextColor / 2, Main.mouseTextColor);
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, nameString, drawPos.X, drawPos.Y, color, Color.Black, Vector2.Zero, 1);

			// Draw the stack amount of the referenced soul.
			Main.LocalPlayer.GetModPlayer<SoulPlayer>().UnlockedSouls.TryGetValue(soulReference.soulNPC, out byte soulStack);
			nameString = "- " + soulStack;

			drawPos.X += this.Width.Pixels - 40;
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, nameString, drawPos.X, drawPos.Y, Color.White, Color.Black, Vector2.Zero, 1);
		}

		public override int CompareTo(object obj)
		{
			if (this.soulReference == null)
				return (-1);
			if (obj == null)
				return (1);
			if (obj is SoulIndexUISoulSlot other)
			{
				if (other.soulReference == null)
					return (1);
				return (soulReference.SoulNPCName().CompareTo(other.soulReference.SoulNPCName()));
			}
			return (1);
		}
	}
}
