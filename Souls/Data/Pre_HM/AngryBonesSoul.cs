using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class AngryBonesSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.AngryBones;
		public override string soulDescription => "Summons a charging skeleton.";

		public override short cooldown => 120;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(10 + 3 * stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = new Vector2(4 * p.direction, 0);
				
			Projectile.NewProjectile(p.Center, velocity, ProjectileType<AngryBonesSoulProj>(), 20 + stack, .1f, p.whoAmI);
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle };
	}

	public class AngryBonesSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_0";

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

			drawOriginOffsetY = -8;
		}

		public override bool PreAI()
		{			
			if (projectile.ai[1] == 0)
				projectile.ai[1] = Utils.SelectRandom<int>(Main.rand, NPCID.AngryBones, NPCID.AngryBonesBig, NPCID.AngryBonesBigMuscle, NPCID.AngryBonesBigHelmet);

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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = GetTexture("Terraria/NPC_" + (int)projectile.ai[1]);
			Vector2 origin = new Vector2(tex.Width / 2, (tex.Height / Main.projFrames[projectile.type]) / 2);
			SpriteEffects effects = projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition + new Vector2(0, drawOriginOffsetY),
				Utils.Frame(tex, 1, 15, 0, projectile.frame), lightColor, projectile.rotation, origin, projectile.scale, effects, 0f);

			return (false);
		}
	}
}
