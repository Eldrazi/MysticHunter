﻿using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class BrainOfCthuluSoul : PreHMSoul, IBossSoul
	{
		public override short soulNPC => NPCID.BrainofCthulhu;
		public override string soulDescription => "Summon protective creepers.";

		public override short cooldown => 3600;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 5 + stack;
			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = p.Center + new Vector2(Main.rand.Next(241) - 120, Main.rand.Next(241) - 120);
				NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, NPCType<BrainOfCthuluSoulNPC>(), 0, p.whoAmI);
			}
			return (true);
		}
	}

	public class BrainOfCthuluSoulNPC : ModNPC
	{
		public override string Texture => "Terraria/NPC_267";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Creeper");
			Main.npcFrameCount[npc.type] = 1;
		}
		public override void SetDefaults()
		{
			npc.width = npc.height = 30;

			npc.lifeMax = 1;
			npc.damage = 20;

			npc.friendly = true;
			npc.noGravity = true;
			npc.noTileCollide = true;

			npc.HitSound = SoundID.NPCHit9;
			npc.DeathSound = SoundID.NPCDeath11;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[(int)npc.ai[0]];

			if (!owner.active)
				npc.active = false;
			if (!owner.dead && owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Blue].soulNPC == NPCID.BrainofCthulhu)
				npc.timeLeft = 2;

			// Set correct NPC velocity.
			Vector2 direction = owner.Center - npc.Center;

			float distance = direction.Length();

			if (distance > 90f)
			{
				distance = 6 / distance;
				direction *= distance;
				npc.velocity = (npc.velocity * 15 + direction) / 16f;
			}
			else
			{
				if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 8f)
				{
					npc.velocity.Y = npc.velocity.Y * 1.05f;
					npc.velocity.X = npc.velocity.X * 1.05f;
				}
			}

			// Check for similar NPCs and adjust velocity to prevent swarming.
			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				if (Main.npc[i].active && Main.npc[i].whoAmI != npc.whoAmI && Main.npc[i].type == npc.type && Main.npc[i].ai[0] == npc.ai[0])
				{
					Vector2 dirToOtherNPC = (Main.npc[i].Center - npc.Center);
					float distToOtherNPC = dirToOtherNPC.Length();

					if (distToOtherNPC <= 10)
						npc.velocity -= Vector2.Normalize(dirToOtherNPC) * .2f;
				}
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Confused, 120);
		public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit) => projectile.Kill();

		public override void NPCLoot()
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Smoke, npc.velocity.X * .2f, npc.velocity.Y * .2f, 100);
		}
	}
}
