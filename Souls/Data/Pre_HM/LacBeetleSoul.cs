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
	public class LacBeetleSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.LacBeetle;
		public override string soulDescription => "Summons a damage shield.";

		public override short cooldown => 180;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			SoulPlayer sp = p.GetModPlayer<SoulPlayer>();

			sp.lacBeetleSoul = !sp.lacBeetleSoul;
			return (true);
		}

		public static readonly PlayerLayer DrawLayer = new PlayerLayer("MysticHunter", "LacBeetle", delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer;
			Texture2D texture = GetTexture("MysticHunter/Souls/Data/Pre_HM/LacBeetleShell");

			int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
			int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
			DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, new Color(86, 27, 99, 100), 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 1, SpriteEffects.None, 0);
			Main.playerDrawData.Add(data);
		});

		/// <summary>
		/// Modifies the given damage value *only* if the damage is melee.
		/// </summary>
		public static void ModifyHit(Player p, ref int damage, PlayerDeathReason damageSource, int stack)
		{
			int damageMod = 5 + 5 * stack;
			int manaConsume = 5 + 2 * stack;

			if (p.CheckMana(manaConsume, true))
			{
				if (damage > damageMod)
					damage -= damageMod;
				else
					damage = 1;
				p.manaRegenDelay = (int)p.maxRegenDelay;
			}
			else
				p.GetModPlayer<SoulPlayer>().cyanBeetleSoul = false;
		}
	}
}
