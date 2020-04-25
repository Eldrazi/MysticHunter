using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class BlueCultistArcherSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.CultistArcherBlue;
		public override string soulDescription => "Summons friendly cultist archers.";

		public override short cooldown => 480;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 20;
		public override bool SoulUpdate(Player p, short stack)
		{
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			Vector2 targetVelocity = Main.MouseWorld;
			for (int i = 0; i < amount; ++i)
			{
				Vector2 spawnPos = p.Center;

				// Hardcoded, since I don't have the good patience to figure it out atm.
				if (amount == 2)
					spawnPos += new Vector2(-30 * (i == 0 ? 1 : -1), 0);
				else if (amount == 3)
					spawnPos += new Vector2(-30 * (1 - i), 0);

				NPC npc = Main.npc[NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, NPCType<BlueCultistArcherSoulNPC>(), 0, p.whoAmI, 0, targetVelocity.X, targetVelocity.Y)];
				npc.damage = 70 + (2 * amount);
				npc.netUpdate = true;
			}

			return (true);
		}
	}

	public class BlueCultistArcherSoulNPC : ModNPC
	{
		public override string Texture => "Terraria/NPC_379";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cultist Archer");
			Main.npcFrameCount[npc.type] = 12;
		}
		public override void SetDefaults()
		{
			npc.width = 18;
			npc.height = 40;

			npc.damage = 0;
			npc.lifeMax = 1;

			npc.friendly = true;
			npc.immortal = true;
		}

		public override bool PreAI()
		{
			if (npc.ai[1] == 0)
				DustEffect();
			else if (npc.ai[1] >= 30)
			{
				if ((int)npc.ai[0] == Main.myPlayer)
				{
					// Spawn the required projectile using the cached `velocity` values in ai[2] and ai[3].
					if (npc.ai[1] == 40)
					{
						Vector2 velocity = Vector2.Normalize(new Vector2(npc.ai[2] - npc.Center.X, npc.ai[3] - npc.Center.Y)) * 8;

						Projectile.NewProjectile(npc.Center, velocity, ProjectileID.WoodenArrowFriendly, npc.damage, .1f, (int)npc.ai[0]);
					}
				}

				// Kill off the NPC.
				if (npc.ai[1] >= 80)
				{
					npc.life = 0;
					DustEffect();
				}
			}

			if (npc.velocity.Y == 0)
				npc.ai[1]++;

			npc.direction = Math.Sign(npc.ai[2] - npc.Center.X);
			npc.spriteDirection = npc.direction;
			return (false);
		}

		public override void FindFrame(int frameHeight)
		{
			Main.NewText(frameHeight);
			if (npc.velocity.Y != 0)
				npc.frame.Y = frameHeight;
			else
			{
				if (npc.ai[1] >= 30 && npc.ai[1] < 50)
				{
					Vector2 rotVec = new Vector2(npc.ai[2] - npc.Center.X, npc.ai[3] - npc.Center.Y);

					if (Math.Abs(rotVec.Y) > Math.Abs(rotVec.X) * 2)
					{
						if (rotVec.Y > 0)
							npc.frame.Y = 2 * frameHeight;
						else
							npc.frame.Y = 6 * frameHeight;
					}
					else if (Math.Abs(rotVec.X) > Math.Abs(rotVec.Y) * 2)
						npc.frame.Y = 4 * frameHeight;
					else if (rotVec.Y > 0)
						npc.frame.Y = 3 * frameHeight;
					else
						npc.frame.Y = 5 * frameHeight;
				}
				else
					npc.frame.Y = 0;
			}
		}

		private void DustEffect()
		{
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default, .8f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
