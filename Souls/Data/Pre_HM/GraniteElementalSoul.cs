using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class GraniteElementalSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.GraniteFlyer;
		public string soulDescription => "Summons a protective granite elemental.";

		public short cooldown => 0;

		public SoulType soulType => SoulType.Blue;

		public short ManaCost(Player p, short stack) => 30;
		public bool SoulUpdate(Player p, short stack)
		{
			// Kill any active GraniteElementalSoulNPCs.
			for (int i = 0; i < Main.maxNPCs; ++i)
				if (Main.npc[i].active && Main.npc[i].ai[0] == p.whoAmI && Main.npc[i].type == NPCType<GraniteElementalSoulNPC>())
					Main.npc[i].active = false;

			// Set some values depending on the stack amount.
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			// Spawn the new elemental(s).
			for (int i = 0; i < amount; ++i)
				NPC.NewNPC((int)p.Center.X, (int)p.Center.Y, NPCType<GraniteElementalSoulNPC>(), 0, p.whoAmI, i, amount);

			return (true);
		}
	}

	/// <summary>
	/// npc.ai[0] = Player/owner ID.
	/// npc.ai[1] = Index of the elemental.
	/// npc.ai[2] = Total amount of elementals.
	/// npc.ai[3] = Rotation/orbit regulator.
	/// </summary>
	public class GraniteElementalSoulNPC : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Elemental Shield");
			Main.npcFrameCount[npc.type] = 12;
		}
		public override void SetDefaults()
		{
			npc.width = 20;
			npc.height = 50;

			npc.lifeMax = 10;
			npc.defense = 0;
			npc.knockBackResist = 0;

			npc.friendly = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[(int)npc.ai[0]];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			// Check to see if the NPC should still be alive.
			if (owner.whoAmI == Main.myPlayer && (owner.dead || sp.souls[(int)SoulType.Blue] == null || sp.souls[(int)SoulType.Blue].soulNPC != NPCID.GraniteFlyer))
					npc.active = false;

			// Start calculating the correct position of the elemental.
			Vector2 newPos = owner.Center + new Vector2(0, 16);

			// We calculate the new position of the NPC based on its given index (npc.ai[1]) and a timer/counter which makes it rotate constantly (npc.ai[2]).
			newPos.X += (float)Math.Cos((MathHelper.TwoPi / npc.ai[2]) * npc.ai[1] + npc.ai[3]) * 50;
			newPos.Y += (float)Math.Sin((MathHelper.TwoPi / npc.ai[2]) * npc.ai[1] + npc.ai[3]) * 50;

			npc.Center = newPos;

			npc.ai[3] += .035f;
			return (false);
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter++;
			if (npc.frameCounter >= 6)
			{
				npc.frame.Y = (npc.frame.Y + frameHeight) % (Main.npcFrameCount[npc.type] * frameHeight);
				npc.frameCounter = 0;
			}
		}

		public override bool CheckDead()
		{
			npc.life = npc.lifeMax;
			return (false);
		}
	}
}
