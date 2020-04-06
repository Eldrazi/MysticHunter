using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Items;
using MysticHunter.Souls.Framework;

namespace MysticHunter
{
	/// <summary>
	/// The sole purpose of this class is that it allows us to drop souls from NPCs that have a soul drop.
	/// </summary>
	public class SoulNPC : GlobalNPC
	{		
		public override void NPCLoot(NPC npc)
		{
			// Soul dropping.
			short snetID = (short)npc.netID;
			if (MysticHunter.Instance.SoulDict.ContainsKey(snetID))
			{
				float modifier = Main.LocalPlayer.GetModPlayer<SoulPlayer>().soulDropModifier[(int)MysticHunter.Instance.SoulDict[snetID].soulType];

				if (Main.rand.NextFloat() <= modifier)
				{
					Item i = Main.item[Item.NewItem(npc.position, ItemType<BasicSoul>(), 1, true, 0, true)];

					// Set the correct ID of the item drop.
					if (i != null)
					{
						BasicSoul bs = i.modItem as BasicSoul;

						bs.soulNPC = snetID;
					}
				}
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
				spawnRate += 5 + 2 * (sp.YellowSoul.stack);
		}
	}
}
