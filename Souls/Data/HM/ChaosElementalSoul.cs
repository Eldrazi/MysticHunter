#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.HM
{
	public class ChaosElementalSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.ChaosElemental;
		public override string soulDescription => "Teleport to a random location when hit.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (internalCooldown > 0)
				internalCooldown--;

			p.GetModPlayer<SoulPlayer>().preHurtModifier += OnHitModifier;
			return (true);
		}

		private int internalCooldown;
		private bool OnHitModifier(Player player, ref int damage, PlayerDeathReason damageSource, byte soulStack)
		{
			if (internalCooldown <= 0 && player.CheckMana(10, true))
			{
				internalCooldown = 300;

				bool canSpawn = false;
				Vector2 newPosition = TestTeleport(player, ref canSpawn, (int)(player.Center.X / 16) - 10, 20, (int)(player.Center.Y / 16) - 10, 20);
				if (canSpawn)
				{
					player.UnityTeleport(newPosition);
					return (false);
				}
			}
			return (true);
		}

		private Vector2 TestTeleport(Player player, ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY)
		{
			int tries = 0;

			int width = player.width;
			Vector2 vector = Vector2.Zero;

			while (!canSpawn && tries < 1000)
			{
				int x = teleportStartX + Main.rand.Next(teleportRangeX);
				int y = teleportStartY + Main.rand.Next(teleportRangeY);
				vector = new Vector2(x, y) * 16f + new Vector2(-width / 2f + 8f, -player.height);
				if (!Collision.SolidCollision(vector, width, player.height) &&
					!Collision.LavaCollision(vector, width, player.height) &&
					Collision.HurtTiles(vector, player.velocity, width, player.height).Y <= 0f)
				{
					if (Main.tile[x, y] == null)
						Main.tile[x, y] = new Tile();

					if ((Main.tile[x, y].wall != 87 || y <= Main.worldSurface || NPC.downedPlantBoss) &&
						(!Main.wallDungeon[Main.tile[x, y].wall] || y <= Main.worldSurface || NPC.downedBoss3))
					{
						canSpawn = true;
						break;
					}
				}

				tries++;
			}

			return vector;
		}
	}
}
