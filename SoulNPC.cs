using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Config;
using MysticHunter.Souls.Items;
using MysticHunter.Souls.Framework;
using System.Linq;

namespace MysticHunter
{
	/// <summary>
	/// The sole purpose of this class is that it allows us to drop souls from NPCs that have a soul drop.
	/// </summary>
	public class SoulNPC : GlobalNPC
	{
		#region Soul Drop Related Logic

		public override void NPCLoot(NPC npc)
		{
			if ((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail) && !npc.boss)
				return;

			// Try to get a value for the current NPC from the Soul Dictionary.
			// If it exists, drop a soul with the `soulNPC` member from the BaseSoul object.
			// This is done in the case the current NPC is an alternate npc for the referenced soul.
			if (MysticHunter.Instance.SoulDict.TryGetValue((short)npc.netID, out BaseSoul soul))
				DropSoulnstanced(soul, npc.position);

			// Pre HM accessory drops.
			if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.BrainofCthulhu)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<BraceOfEvil>());
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<OcularCharm>());
			}
			else if (npc.type == NPCID.SkeletronHead)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<CursedHand>());
			}
			else if (npc.type == NPCID.QueenBee)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<QueenKnuckle>());
			}
			else if (npc.type == NPCID.WallofFlesh)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem(npc.position, ItemType<SoulOfTheDamned>());
			}
			// Post HM accessory drops.
			else if (npc.type == NPCID.TheDestroyer)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<MechanoScarf>());
			}
			else if (npc.type == NPCID.SkeletronPrime)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<MechanizedBoneNecklace>());
			}
			else if ((npc.type == NPCID.Retinazer && !NPC.AnyNPCs(NPCID.Spazmatism)) ||
				(npc.type == NPCID.Spazmatism && !NPC.AnyNPCs(NPCID.Retinazer)))
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<BandOfDoubleSight>());
			}
			else if (npc.type == NPCID.Plantera)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<LivingGloves>());
			}
			else if (npc.type == NPCID.Golem)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<GolematicBoots>());
			}
			else if (npc.type == NPCID.DukeFishron)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<FishronSteak>());
			}
			else if (npc.type == NPCID.CultistBoss)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem(npc.position, ItemType<LunarRitual>());
			}
			else if (npc.type == NPCID.MoonLordCore)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem(npc.position, ItemType<LuminiteRing>());
			}
		}

		/// <summary>
		/// Tries to drop a soul instance for the specified NPC type.
		/// If the player is in SinglePlayer mode, a local item is created.
		/// In multiplayer mode, the server creates an instanced item and sends a net message to the relevant clients.
		/// </summary>
		/// <param name="soul">The <see cref="BaseSoul"/> instance for the current npc.</param>
		/// <param name="position">The positition to spawn the soul at.</param>
		public static void DropSoulnstanced(BaseSoul soul, Vector2 position)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				var serverConfig = ModContent.GetInstance<SoulServerConfig>();

				int item = Item.NewItem(position, ItemType<BasicSoulItem>(), 1, noBroadcast: true);
				BasicSoulItem bs = Main.item[item].modItem as BasicSoulItem;
				bs.soulNPC = soul.soulNPC;

				Main.itemLockoutTime[item] = 54000;
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active)
					{
						float modifier = Main.player[i].GetModPlayer<SoulPlayer>().soulDropModifier[(int)soul.soulType];
						if (Main.rand.NextFloat() <= modifier || serverConfig.GuaranteedSoulDrops.Any(x => x.Type == soul.soulNPC))
						{
							NetMessage.SendData(MessageID.InstancedItem, i, -1, null, item);
						}
					}
				}
				Main.item[item].active = false;
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				var serverConfig = ModContent.GetInstance<SoulServerConfig>();
				float modifier = Main.LocalPlayer.GetModPlayer<SoulPlayer>().soulDropModifier[(int)soul.soulType];

				if (Main.rand.NextFloat() <= modifier || serverConfig.GuaranteedSoulDrops.Any(x => x.Type == soul.soulNPC))
				{
					Item item = Main.item[Item.NewItem(position, ItemType<BasicSoulItem>())];
					if (item != null)
					{
						BasicSoulItem bs = item.modItem as BasicSoulItem;
						bs.soulNPC = soul.soulNPC;
					}
				}
			}
		}

		#endregion

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			SoulPlayer sp = player.GetModPlayer<SoulPlayer>();

			if (sp.pinkySoul)
			{
				spawnRate -= 5 * (sp.YellowSoulNet.stack);
				if (spawnRate < 5)
					spawnRate = 5;
			}
		}

		public override bool PreAI(NPC npc)
		{
			if (npc.HasBuff(BuffID.Slow))
			{
				npc.wet = true;
			}

			return (true);
		}
	}
}
