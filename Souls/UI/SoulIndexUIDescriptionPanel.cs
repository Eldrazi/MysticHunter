#region Using directives

using Terraria;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.UI
{
	internal class SoulIndexUIDescriptionPanel : GenericUIPanel
	{
		private NPC drawNPC;
		public BaseSoul soulReference;

		public SoulIndexUIDescriptionPanel(Texture2D panelTexture, Vector2 panelDimensions) : base(panelTexture, panelDimensions) { }

		public override void OnInitialize()
		{
			this.SetPadding(0);

			this.Top.Pixels = 0;
			this.Left.Pixels = this.Parent.Width.Pixels / 2 + 22;

			this.Width.Pixels = this.Parent.Width.Pixels / 2 - 28;
			this.Height.Pixels = this.Parent.Height.Pixels - SoulIndexUIListPanel.height - 8;

			drawNPC = new NPC();
			this.soulReference = null;
		}

		public void SetSoulReference(BaseSoul soul)
		{
			this.soulReference = soul;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			// Draw the base panel.
			base.DrawSelf(spriteBatch);

			if (this.soulReference == null || this.soulReference.soulNPC == 0)
				return;

			if (drawNPC.netID != this.soulReference.soulNPC)
				drawNPC.SetDefaults(this.soulReference.soulNPC);

			Main.instance.LoadNPC(drawNPC.type);

			Vector2 drawPos = this.GetDimensions().Position();

			// Draw the name of the NPC.
			drawPos += Vector2.One * 16;
			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, this.soulReference.SoulNPCName(), drawPos.X, drawPos.Y, Color.White, Color.Black, Vector2.Zero, 1);

			// Draw the image of the NPC.
			float npcScale = 1;
			float maxNPCSize = 50;
			Texture2D npcTexture = TextureAssets.Npc[drawNPC.type].Value;
			Rectangle npcRectangle = new Rectangle(0, 0, npcTexture.Width, npcTexture.Height / Main.npcFrameCount[drawNPC.type]);

			if (npcRectangle.Width > maxNPCSize || npcRectangle.Height > maxNPCSize)
			{
				if (npcRectangle.Width > npcRectangle.Height)
					npcScale = maxNPCSize / npcRectangle.Width;
				else
					npcScale = maxNPCSize / npcRectangle.Height;
			}

			drawPos.Y += 20;
			// Draw NPC centralized in its own little space, determined by the maxNPCSize variable.
			spriteBatch.Draw(npcTexture,
				drawPos + new Vector2(maxNPCSize / 2 - (npcRectangle.Width * npcScale) / 2, maxNPCSize / 2 - (npcRectangle.Height * npcScale) / 2),
				npcRectangle, drawNPC.color != default ? drawNPC.color : Color.White, 0, Vector2.Zero, npcScale, SpriteEffects.FlipHorizontally, 0f);

			// Draw npc id.
			string npcTypeString = "no. " + soulReference.soulNPC;
			Vector2 tmpDrawPos = drawPos + new Vector2(maxNPCSize + 16, maxNPCSize / 2 - 9);
			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, npcTypeString, tmpDrawPos.X, tmpDrawPos.Y, Color.White, Color.Black, Vector2.Zero, 1);

			// Draw the description of the NPC's soul.
			drawPos.Y += maxNPCSize + 4;
			string[] snippets = Utils.WordwrapString(soulReference.soulDescription, FontAssets.MouseText.Value, (int)this.Width.Pixels, 10, out _);
			for (int i = 0; i < snippets.Length; ++i)
			{
				if (string.IsNullOrEmpty(snippets[i]))
					break;

				int stringWidth = (int)FontAssets.ItemStack.Value.MeasureString(snippets[i]).X ;
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, snippets[i],
					drawPos.X + ((this.Width.Pixels / 2) * .9f - (stringWidth / 2)),
					drawPos.Y, Color.White, Color.Black, Vector2.Zero, .9f);
				drawPos.Y += 20;
			}
		}
	}
}
