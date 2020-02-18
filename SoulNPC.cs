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
			short snetID = (short)npc.netID;
			if (MysticHunter.Instance.SoulDict.ContainsKey(snetID))
			{
				// TODO
				// There needs to be a random check here, since we don't want souls to *always* drop.
				// Leaving it like this is solely for debugging purposes.

				Item i = Main.item[Item.NewItem(npc.position, ItemType<BasicSoul>(), 1, true)];

				// Set the correct ID of the item drop.
				if (i != null)
				{
					BasicSoul bs = i.modItem as BasicSoul;

					bs.soulNPC = snetID;
				}
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();

			if (sp.pinkySoul)
				spawnRate += 5 + 2 * (sp.soulsStack[0]);
		}
	}
}
