using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class MothSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Moth;
		public override string soulDescription => "Spawns deadly dust when flying.";

		public override short cooldown => 20;

		public override SoulType soulType => SoulType.Yellow;

		public override short ManaCost(Player p, short stack) => 0;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Player is flying.
			if (p.wingTime != p.wingTimeMax)
			{
				int randomProjType = Main.rand.Next(3);
				int randomProjFrame = Main.rand.Next(3);
				Projectile.NewProjectile(p.Center, new Vector2(p.velocity.X * .2f, 2), ProjectileType<MothSoulProj>(), 10, 0, p.whoAmI, randomProjType, stack);
			}
			return (true);
		}
	}

	public class MothSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Dust";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Deadly Dust");
			Main.projFrames[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 6;

			projectile.scale = 1.5f;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;

			projectile.magic = true;
			projectile.friendly = true;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			float maxXSpeed = 2;
			float acceleration = .04f;

			if (projectile.localAI[1] == 0)
				projectile.localAI[1] = Main.rand.Next(1, 4);

			if (projectile.localAI[0]++ <= 60)
			{
				if (projectile.velocity.X < 0)
					projectile.velocity.X *= .98f;
				if (projectile.velocity.X < maxXSpeed)
					projectile.velocity.X += acceleration;
				else
					projectile.velocity.X *= .95f;
			}
			else
			{
				if (projectile.velocity.X > 0)
					projectile.velocity.X *= .98f;
				if (projectile.velocity.X > -maxXSpeed)
					projectile.velocity.X -= acceleration;
				else
					projectile.velocity.X *= .95f;

				if (projectile.localAI[0] >= 120)
					projectile.localAI[0] = 0;
			}

			projectile.rotation += projectile.velocity.X * .2f;

			Lighting.AddLight(projectile.Center, .2f, .2f, .2f);
			return (false);
		}

		private readonly int[] buffs = new int[] { BuffID.Venom, BuffID.Frostburn, BuffID.OnFire };
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int duration = 0;
			if (projectile.ai[1] >= 5)
				duration += 60;
			if (projectile.ai[1] >= 9)
				duration += 60;

			if (duration != 0 && Main.rand.Next(4) == 0)
				target.AddBuff(buffs[(int)projectile.ai[0]], duration);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = GetTexture(Texture);

			int frameIndex = 15;
			if (projectile.ai[0] == 1)
				frameIndex = 57;
			else if (projectile.ai[0] == 2)
				frameIndex = 58;

			Vector2 projOrigin = new Vector2(5, 5);
			Rectangle projectileFrame = new Rectangle(
				frameIndex * 10 % projTexture.Width,
				frameIndex * 10 / projTexture.Width + (int)((projectile.localAI[1] - 1) * 10),
				10, 10);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, projectileFrame,	lightColor * projectile.Opacity, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}
	}
}
