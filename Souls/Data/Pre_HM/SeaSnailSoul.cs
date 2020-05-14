using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class SeaSnailSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.SeaSnail;
		public override string soulDescription => "Wear a thorny snail house.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			SoulPlayer sp = p.GetModPlayer<SoulPlayer>();

			sp.seaSnailSoul = !sp.seaSnailSoul;
			return (true);
		}

		public override void PostUpdate(Player player)
		{
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();
			if (!sp.seaSnailSoul)
				return;

			player.statDefense += sp.activeSouls[(int)SoulType.Blue].stack;
			player.thorns += (.15f + .05f * sp.activeSouls[(int)SoulType.Blue].stack);

			if (sp.seaSnailAnimationCounter < 24)
				sp.seaSnailAnimationCounter++;
		}

		public static readonly PlayerLayer DrawLayer = new PlayerLayer("MysticHunter", "SeaSnail", delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer;
			Texture2D texture = GetTexture("MysticHunter/Souls/Data/Pre_HM/SeaSnailShell");

			int frame = drawPlayer.GetModPlayer<SoulPlayer>().seaSnailAnimationCounter / 3;
			Rectangle animationFrame = new Rectangle(0, frame * 32, 32, 32);

			SpriteEffects effects = drawPlayer.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Vector2 drawPosition = drawInfo.position - Main.screenPosition;
			drawPosition.X += (-animationFrame.Width * .5f) + (drawPlayer.width * .5f) - (drawPlayer.direction == 1 ? 2 : -2);
			drawPosition.Y += (drawPlayer.height - drawPlayer.bodyFrame.Height) - 4;
			drawPosition += drawPlayer.headPosition + drawInfo.headOrigin;

			drawPosition.X = (int)drawPosition.X;
			drawPosition.Y = (int)drawPosition.Y;

			DrawData data = new DrawData(texture, drawPosition, animationFrame, drawInfo.upperArmorColor, drawPlayer.headRotation, drawInfo.headOrigin, 1, effects, 0);
			Main.playerDrawData.Add(data);
		});
	}
}
