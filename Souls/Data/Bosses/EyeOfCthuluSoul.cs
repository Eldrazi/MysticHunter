using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class EyeOfCthuluSoul : BaseSoul
	{
		public override short soulNPC => NPCID.EyeofCthulhu;
		public override string soulDescription => "Dash through enemies.";

		public override short cooldown => 60;//900;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			if (p.mount.Active)
				p.mount.Dismount(p);

			this.dashTime = 30;
			p.GetModPlayer<SoulPlayer>().eocSoulDash = true;
			p.velocity.X = p.direction * 14f;

			Main.PlaySound(15, p.Center, 0);
			return (true);
		}

		private int dashTime = 0;
		public override void PostUpdate(Player player)
		{
			if (player.GetModPlayer<SoulPlayer>().eocSoulDash && dashTime > 0)
			{
				Rectangle rectangle = new Rectangle((int)(player.position.X + player.velocity.X * 0.5 - 4.0), (int)(player.position.Y + player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC npc = Main.npc[i];
					if (npc.active && !npc.dontTakeDamage && !npc.friendly)
					{
						Rectangle rect = npc.getRect();
						if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
						{
							bool crit = false;
							float knockback = 9f;
							float damage = (25 + 3 * this.stack) * player.meleeDamage;

							if (player.kbGlove)
								knockback *= 2;
							if (player.kbBuff)
								knockback *= 1.5f;

							if (Main.rand.Next(100) < player.meleeCrit)
								crit = true;

							int dir = player.direction;
							if (player.velocity.X < 0)
								dir = -1;
							if (player.velocity.X > 0)
								dir = 1;

							if (player.whoAmI == Main.myPlayer)
								player.ApplyDamageToNPC(npc, (int)damage, knockback, dir, crit);

							// Make sure the EoC shield dash doesn't stack.
							player.eocDash = 10;
							player.dashDelay = 30;
							player.immune = true;
							player.immuneNoBlink = true;
							player.immuneTime = 4;
						}
					}
				}
			}

			if (dashTime > 0)
			{
				dashTime--;
				if (dashTime <= 0)
					player.GetModPlayer<SoulPlayer>().eocSoulDash = false;
			}
		}

		public static readonly PlayerLayer DrawLayer = new PlayerLayer("MysticHunter", "EyeOfCthulu", delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
				return;

			Player drawPlayer = drawInfo.drawPlayer;
			if (!drawPlayer.GetModPlayer<SoulPlayer>().eocSoulDash)
				return;

			Texture2D texture = GetTexture("MysticHunter/Souls/Data/Bosses/EyeOfCthuluSoul");
			SpriteEffects effects = drawPlayer.velocity.X >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X) - (16 * System.Math.Sign(drawPlayer.velocity.X));
			int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
			DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White * .4f, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 1, effects, 0);
			Main.playerDrawData.Add(data);
		});
	}
}
