using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using IL.Terraria.Audio;

namespace MysticHunter.Souls.Data.Pre_HM
{
	// TODO: Add custom sprite to projectile to make it look better.
	public class GiantShellySoul : PreHMSoul
	{
		public override short soulNPC => NPCID.GiantShelly;
		public override string soulDescription => "Summons a bouncing shell.";

		public override short cooldown => 240;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 15;
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);

			Projectile.NewProjectile(p.Center, velocity, ProjectileType<GiantShellySoulProj>(), 15 + stack, .1f, p.whoAmI, 2 + stack);
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.GiantShelly2 };
	}

	public class GiantShellySoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_496";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Shell");
			Main.projFrames[projectile.type] = 12;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.penetrate = -1;

			projectile.melee = true;
			projectile.friendly = true;

			drawOriginOffsetY = -4;
		}

		public override bool PreAI()
		{
			projectile.frame = 7;
			projectile.spriteDirection = 1;
			projectile.rotation = - .4f;

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				if (projectile.ai[0] <= 0)
					return (true);

				projectile.ai[0]--;
				projectile.velocity.X = -oldVelocity.X;
				Main.PlaySound(SoundID.Dig, projectile.position);
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; ++i)
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
