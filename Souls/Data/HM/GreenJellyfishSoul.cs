using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.HM
{
	public class GreenJellyfishSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.GreenJellyfish;
		public override string soulDescription => "Summon an electric shield.";

		public override short cooldown => 1800;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 25;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 2 + (int)(.5f * stack);
			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<GreenJellyfishSoulProj>(), damage, .2f, p.whoAmI, stack);

			return (true);
		}
	}

	public class GreenJellyfishSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Projectile_443";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Electric Shield");
			Main.projFrames[projectile.type] = 4;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 102;

			projectile.alpha = 100;
			projectile.penetrate = -1;
			projectile.timeLeft = 600;

			projectile.friendly = true;
			projectile.tileCollide = false;

			projectile.localNPCHitCooldown = 6;
			projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.dead)
				projectile.Kill();

			// Correct projectile positioning on the owner.
			projectile.position = owner.Center - new Vector2(projectile.width * .5f, projectile.height * .5f);

			// Visual effects.
			projectile.rotation += .02f;
			if (projectile.frameCounter++ >= 2)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Electric, 0, 0, 100, default, .6f);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D projTexture = GetTexture(Texture);

			Vector2 projOrigin = new Vector2(projTexture.Width * .5f, (projTexture.Height / Main.projFrames[projectile.type]) * .5f);

			spriteBatch.Draw(projTexture, projectile.Center - Main.screenPosition, new Rectangle?(Utils.Frame(projTexture, 1, 4, 0, projectile.frame)),
				lightColor, projectile.rotation, projOrigin, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}
	}
}
