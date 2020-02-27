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
	public class CyanBeetleSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.CyanBeetle;
		public string soulDescription => "Summons ranged damage shield.";

		public short cooldown => 180;

		public SoulType soulType => SoulType.Blue;

		public short ManaCost(Player p, short stack) => 0;
		public bool SoulUpdate(Player p, short stack)
		{
			SoulPlayer sp = p.GetModPlayer<SoulPlayer>();

			sp.cyanBeetleSoul = !sp.cyanBeetleSoul;
			return (true);
		}

		public static readonly PlayerLayer DrawLayer = new PlayerLayer("MysticHunter", "CyanBeetle", delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer;
			Texture2D texture = GetTexture("MysticHunter/Souls/Data/Pre_HM/CyanBeetleShell");

			int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
			int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
			DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, new Color(34, 73, 128, 100), 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 1, SpriteEffects.None, 0);
			Main.playerDrawData.Add(data);
		});

		/// <summary>
		/// Modifies the given damage value *only* if the damage is melee.
		/// </summary>
		public static void ModifyHit(Player p, ref int damage, PlayerDeathReason damageSource, int stack)
		{
			int damageMod = 5 + 5 * stack;
			int manaConsume = 5 + 2 * stack;
			if (damageSource.SourceProjectileType != 0)
			{
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
}
