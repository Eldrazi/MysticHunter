using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Framework;

using Microsoft.Xna.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class DarkCasterSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.DarkCaster;
		public override string soulDescription => "Summons a bolt of cursed water.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(15 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center);
			velocity *= 6;

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<DarkCasterSoulProj>(), 18 + 2 * stack, .1f + .01f * stack, p.whoAmI);
			return (true);
		}
	}

	public class DarkCasterSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_33";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Water");
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;
			projectile.alpha = 255;

			projectile.penetrate = -1;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0] == 0)
			{
				projectile.ai[0] = 1;
				Main.PlaySound(SoundID.Item8, projectile.position);
			}

			// Spawn a dust trail.
			for (int i = 0; i < 2; i++)
			{
				for (int j = i; j < 3; j++)
				{
					float xMod = projectile.velocity.X / 3f * i;
					float yMod = projectile.velocity.Y / 3f * i;

					Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X + 2, projectile.position.Y + 2), projectile.width - 4, projectile.height - 2 * 2, 172, 0f, 0f, 100, default, 1.2f)];
					d.noGravity = true;
					d.velocity = (d.velocity * .1f) + projectile.velocity * .5f;
					d.position.X -= xMod;
					d.position.Y -= yMod;
				}
				if (Main.rand.Next(5) == 0)
				{
					Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X + 2, projectile.position.Y + 2), projectile.width - 4, projectile.height - 2 * 2, 172, 0f, 0f, 100, default, .6f)];
					d.velocity = (d.velocity * .25f) + projectile.velocity * .5f;
				}
			}
			projectile.rotation += 0.4f * projectile.direction;
			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (oldVelocity.X != projectile.velocity.X)
				projectile.velocity.X = -oldVelocity.X;
			if (oldVelocity.Y != projectile.velocity.Y)
				projectile.velocity.Y = -oldVelocity.Y;

			Main.PlaySound(SoundID.Item10, projectile.position);
			return (false);
		}
	}
}
