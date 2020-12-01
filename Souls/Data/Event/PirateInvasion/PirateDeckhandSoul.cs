using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Event.PirateInvasion
{
	public class PirateDeckhandSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.PirateDeckhand;
		public override string soulDescription => "Summons a Pirate Deckhand to charge at foes and steal their coin.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 10;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 30;
			int modifier = 0;

			if (stack >= 5)
			{
				damage += 20;
				modifier++;
			}
			if (stack >= 9)
			{
				damage += 20;
				modifier++;
			}

			Vector2 velocity = new Vector2(4 * p.direction, 0);
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<PirateDeckhandSoulProj>(), damage, .5f, p.whoAmI, 0, modifier);

			return (true);
		}
	}

	public class PirateDeckhandSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.PirateDeckhand;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pirate Deckhand");
			Main.projFrames[projectile.type] = 16;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.melee = true;
			projectile.friendly = true;

			projectile.penetrate = 3;

			drawOffsetX = -6;
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
					projectile.frame = 1;
				}
				else
				{
					projectile.rotation = projectile.velocity.X * .01f;

					projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];

					if (projectile.frame < 2)
						projectile.frame = 2;
				}
			}
			else
			{
				projectile.frame = 1;
				projectile.rotation -= projectile.spriteDirection * .03f;
			}

			projectile.velocity.Y += .2f;
			return (false);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return (true);
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

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.owner == Main.myPlayer && target.CanBeChasedBy(projectile))
			{
				int[] coinTypes = { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin };
				Item.NewItem(target.Hitbox, coinTypes[(int)projectile.ai[1]]);
			}
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
