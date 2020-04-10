using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Items;
using MysticHunter.Souls.Framework;

namespace MysticHunter
{
	/// <summary>
	/// The sole purpose of this class is that it allows us to drop souls from NPCs that have a soul drop.
	/// </summary>
	public class SoulNPC : GlobalNPC
	{
		public static short lastKilledSoulNPC;

		public override void NPCLoot(NPC npc)
		{
			// If the player is playing in singleplayer, call TryDropNPCSoul directly.
			// Otherwise the 'player' is a server and should send a message to all connected clients.
			if (MysticHunter.Instance.SoulDict.ContainsKey((short)npc.netID))
			{
				DropSoulnstanced((short)npc.netID, npc.position);
			}

			if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.BrainofCthulhu)
			{
				if (Main.rand.Next(50) == 0)
					Item.NewItem(npc.position, ItemType<BraceOfEvil>());
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				if (Main.rand.Next(50) == 0)
					Item.NewItem(npc.position, ItemType<OccularCharm>());
			}
			else if (npc.type == NPCID.SkeletronHead)
			{
				if (Main.rand.Next(50) == 0)
					Item.NewItem(npc.position, ItemType<CursedHand>());
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();

			if (sp.pinkySoul)
				spawnRate += 5 + 2 * (sp.UnlockedSouls[sp.YellowSoul.soulNPC]);
		}

		/// <summary>
		/// Tries to drop a soul instance for the specified NPC type.
		/// If the player is in SinglePlayer mode, a local item is created.
		/// In multiplayer mode, the server creates an instanced item and sends a net message to the relevant clients.
		/// </summary>
		/// <param name="npcType">The type of NPC for which the soul needs to spawn.</param>
		/// <param name="position">The positition to spawn the soul at.</param>
		public static void DropSoulnstanced(short npcType, Vector2 position)
		{
			if (Main.netMode == 2)
			{
				int item = Item.NewItem(position, ItemType<BasicSoul>(), 1, noBroadcast: true);
				BasicSoul bs = Main.item[item].modItem as BasicSoul;
				bs.soulNPC = npcType;

				Main.itemLockoutTime[item] = 54000;
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active)
					{
						float modifier = Main.player[i].GetModPlayer<SoulPlayer>().soulDropModifier[(int)MysticHunter.Instance.SoulDict[npcType].soulType];
						if (Main.rand.NextFloat() <= modifier)
						{
							NetMessage.SendData(90, i, -1, null, item);
						}
					}
				}
				Main.item[item].active = false;
			}
			else if (Main.netMode == 0)
			{
				float modifier = Main.LocalPlayer.GetModPlayer<SoulPlayer>().soulDropModifier[(int)MysticHunter.Instance.SoulDict[npcType].soulType];
				if (Main.rand.NextFloat() <= modifier)
				{
					Item item = Main.item[Item.NewItem(position, ItemType<BasicSoul>())];
					if (item != null)
					{
						BasicSoul bs = item.modItem as BasicSoul;
						bs.soulNPC = npcType;
					}
				}
			}
		}
	}
}
