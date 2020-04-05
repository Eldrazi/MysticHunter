using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class AngryBonesSoul : ISoul
	{
		public bool acquired { get; set; }

		public short soulNPC => NPCID.AngryBones;
		public string soulDescription => "Summons a charging skeleton.";

		public short cooldown => 120;

		public SoulType soulType => SoulType.Red;

		public short ManaCost(Player p, short stack) => (short)(10 + 3 * stack);
		public bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);
				
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<AngryBonesSoulProj>(), 20 + stack, .1f, p.whoAmI);
			return (true);
		}
	}

	public class AngryBonesSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_31";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skeleton");
			Main.projFrames[projectile.type] = 15;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.melee = true;
			projectile.friendly = true;

			projectile.penetrate = 3;

			drawOriginOffsetY = -20;
		}

		public override bool PreAI()
		{
			if (projectile.ai[0] == 0)
			{
				// Set the correct direction of the projectile.
				if (projectile.velocity.X > 0)
					projectile.spriteDirection = -1;
				else
					projectile.spriteDirection = 1;

				// Animate the projectile.
				if (projectile.velocity.Y != 0)
				{
					projectile.frame = 0;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];

					if (projectile.frame == 0)
						projectile.frame = 1;
				}
			}
			else
			{
				projectile.frame = 0;
				projectile.rotation -= projectile.spriteDirection * .03f;
			}

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.ai[0] = 1;
				projectile.timeLeft = 30;
				projectile.netUpdate = true;
				projectile.tileCollide = false;

				projectile.velocity.Y = -2;
				projectile.velocity.X = oldVelocity.X * .5f;
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
