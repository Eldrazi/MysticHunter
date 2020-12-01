using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.MartianMadness
{
	public class MartianOfficerSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.MartianOfficer;
		public override string soulDescription => "Summon a mix of friendly martians.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 70 + 10 * stack;

			Vector2 spawnPos = Main.MouseWorld;

			if (Collision.SolidCollision(spawnPos, 16, 16))
				return (false);

			Projectile.NewProjectile(spawnPos, Vector2.Zero, ProjectileType<MartianOfficerSoulProj>(), damage, .5f, p.whoAmI);

			return (true);
		}
	}

	internal sealed class MartianOfficerSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_" + ProjectileID.None;

		private int[] projectileTextureTypes = { NPCID.MartianEngineer, NPCID.MartianOfficer, NPCID.GigaZapper, NPCID.GrayGrunt, NPCID.BrainScrambler };

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Martian");
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 24;
			
			projectile.scale = .8f;
			projectile.alpha = 255;
			projectile.penetrate = 1;

			projectile.magic = true;
			projectile.friendly = true;

			drawOffsetX = -32;
			drawOriginOffsetY = -18;
		}

		public override bool PreAI()
		{
			if (projectile.localAI[0] == 0)
			{
				SpawnDust();
				projectile.frame = 1;
				projectile.localAI[0] = projectileTextureTypes[Main.rand.Next(projectileTextureTypes.Length)];
			}

			projectile.velocity.Y += .2f;
			projectile.rotation -= projectile.velocity.Y * .02f;

			return (false);
		}

		public override void Kill(int timeLeft) => SpawnDust();

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Main.instance.LoadNPC((int)projectile.localAI[0]);
			Texture2D texture = Main.npcTexture[(int)projectile.localAI[0]];
			Rectangle frame = texture.Frame(1, Main.npcFrameCount[(int)projectile.localAI[0]], 0, projectile.frame);
			Vector2 origin = frame.Size() / 2;

			spriteBatch.Draw(texture, projectile.position + origin - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}

		private void SpawnDust()
		{
			for (int i = 0; i < 10; ++i)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1f)];
				if (Main.rand.Next(2) == 0)
				{
					d.scale = .5f;
					d.fadeIn = 1 + Main.rand.Next(10) * .1f;
				}
			}
		}
	}
}
