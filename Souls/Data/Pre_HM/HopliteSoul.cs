using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class HopliteSoul : BaseSoul
	{
		public override short soulNPC => NPCID.GreekSkeleton;
		public override string soulDescription => "Summons a hoplite.";

		public override short cooldown => 480;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 25;
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

				NPC npc = Main.npc[NPC.NewNPC((int)spawnPos.X, (int)spawnPos.Y, NPCType<HopliteSoulNPC>(), 0, p.whoAmI, 0, targetVelocity.X, targetVelocity.Y)];
				npc.damage = 20 + (5 * amount);
				npc.netUpdate = true;
			}

			return (true);
		}
	}

	public class HopliteSoulNPC : ModNPC
	{
		public override string Texture => "Terraria/NPC_481";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hoplite");
			Main.npcFrameCount[npc.type] = 19;
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

						Projectile.NewProjectile(npc.Center, velocity, ProjectileID.JavelinFriendly, npc.damage, .1f, (int)npc.ai[0]);
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
			if (npc.velocity.Y != 0)
				npc.frame.Y = 0;
			else
			{
				if (npc.ai[1] >= 30 && npc.ai[1] < 50)
				{
					int frame = (int)Math.Floor((npc.ai[1] - 30) / 5);
					npc.frame.Y = (15 + frame) * frameHeight;
				}
				else
					npc.frame.Y = frameHeight;
			}
		}

		private void DustEffect()
		{
			for (int i = 0; i < 20; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
