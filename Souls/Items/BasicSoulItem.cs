using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MysticHunter.Souls.Items
{
	/// <summary>
	/// The basic class for a soul item.
	/// </summary>
	public class BasicSoulItem : ModItem
	{
		public short soulNPC = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul");
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}
		public override void SetDefaults()
		{
			item.width = item.height = 16;
		}

		private readonly string[] numberList = new string[] { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth" };

		/// <summary>
		/// Triggers on pickup. Checks if a corresponding soul exists and sets that soul as acquired.
		/// </summary>
		/// <param name="player">The player that interacts with this item.</param>
		/// <returns>Always returns false. This item is not actually added to the players' inventory.</returns>
		public override bool OnPickup(Player player)
		{
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();

			if (MysticHunter.Instance.SoulDict.ContainsKey(soulNPC))
			{
				BaseSoul s = MysticHunter.Instance.SoulDict[soulNPC];

				Color c = Color.Red;
				if (s.soulType == SoulType.Blue)
					c = Color.Blue;
				else if (s.soulType == SoulType.Yellow)
					c = Color.Yellow;

				if (!sp.UnlockedSouls.ContainsKey(soulNPC))
					sp.UnlockedSouls.Add(soulNPC, 0);

				if (sp.UnlockedSouls[soulNPC] < 9)
				{
					// Display a message in chat.
					Main.NewText($"You collected your {numberList[sp.UnlockedSouls[soulNPC]]} {s.SoulNPCName()} soul.", c);

					// Increase the stack for this soul.
					sp.UnlockedSouls[soulNPC]++;

					// Update the local player and UI.
					sp.UpdateActiveSoulData();
					SoulManager.ReloadSoulIndexUI();

					// Emit a sound a particle effect.
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/SoulPickup"), player.Center);
					for (int i = 0; i < 5; ++i)
					{
						Dust d = Main.dust[Dust.NewDust(player.position, player.width, player.height, DustID.AncientLight, 0f, 0f, 255, c, Main.rand.Next(20, 26) * 0.1f)];
						d.noLight = true;
						d.noGravity = true;
						d.velocity *= 0.5f;
					}
				}
			}
			else
			{
				// Debug message for when a soul with the given `soulDataID` cannot be found.
				Main.NewText("Soul with ID '" + soulNPC + "' cannot be found.");
			}

			return (false);
		}

		public override bool ItemSpace(Player player) => true;

		public override void GrabRange(Player player, ref int grabRange)
			=> grabRange += 50;

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (soulNPC == 0)
				return;

			BaseSoul s = MysticHunter.Instance.SoulDict[soulNPC];

			Vector3 c = new Vector3(.6f, .3f, .2f);
			if (s.soulType == SoulType.Blue)
				c = new Vector3(.2f, .6f, .3f);
			else if (s.soulType == SoulType.Yellow)
				c = new Vector3(.2f, .5f, .5f);

			Lighting.AddLight(item.position, c);
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (MysticHunter.Instance.SoulDict.ContainsKey(soulNPC))
			{
				SoulType type = MysticHunter.Instance.SoulDict[soulNPC].soulType;

				// Get the correct texture.
				Texture2D tex = mod.GetTexture("Souls/Items/" + type + "Soul");

				// Animate the item.
				if (Main.itemFrameCounter[whoAmI]++ > 5)
				{
					Main.itemFrameCounter[whoAmI] = 0;
					Main.itemFrame[whoAmI] = (Main.itemFrame[whoAmI] + 1) % 4;
				}

				// Draw the item correctly.
				Rectangle rect = tex.Frame(1, 4, 0, Main.itemFrame[whoAmI]);
				Vector2 origin = new Vector2(tex.Width * .5f, (tex.Height / 4) * .5f);

				spriteBatch.Draw(tex, item.position - Main.screenPosition + origin, rect, lightColor, rotation, origin, scale, SpriteEffects.None, 0);
			}
			return (false);
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(this.soulNPC);
		}
		public override void NetRecieve(BinaryReader reader)
		{
			this.soulNPC = reader.ReadInt16();
		}
	}
}
