﻿#region Using directives

using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class DemonEyeSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.DemonEye;
		public override string soulDescription => "Fires a rebounding eyeball.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => (short)(5 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6f;

			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<DemonEyeSoulProj>(), 5 + stack, .1f, p.whoAmI, 1 + (stack / 2));
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.DemonEye2, NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship };
	}

	public class DemonEyeSoulProj : ModProjectile
	{
		public override string Texture => "Terraria/Images/NPC_" + NPCID.DemonEye;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eyeball");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;

			projectile.scale = .8f;
			projectile.penetrate = -1;

			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			// Returning to player AI stage.
			if (projectile.ai[1] == 1)
			{
				projectile.tileCollide = false;

				float speed = 8f;
				float acceleration = .2f;

				Vector2 targetDir = owner.Center - projectile.Center;
				targetDir *= (speed / targetDir.Length());
				if (projectile.velocity.X < targetDir.X)
					projectile.velocity.X += acceleration * ((projectile.velocity.X < 0f && targetDir.X > 0f) ? 2 : 1);
				else if (projectile.velocity.X > targetDir.X)
					projectile.velocity.X -= acceleration * ((projectile.velocity.X > 0f && targetDir.X < 0f) ? 2 : 1);

				if (projectile.velocity.Y < targetDir.Y)
					projectile.velocity.Y += acceleration * ((projectile.velocity.Y < 0f && targetDir.Y > 0f) ? 2 : 1);
				else if (projectile.velocity.Y > targetDir.Y)
					projectile.velocity.Y -= acceleration * ((projectile.velocity.Y > 0f && targetDir.Y < 0f) ? 2 : 1);

				if (owner.whoAmI == Main.myPlayer && projectile.Hitbox.Intersects(owner.Hitbox))
					projectile.Kill();
			}
			else
			{
				if (projectile.localAI[0]++ >= 180)
				{
					projectile.ai[1] = 1;
					projectile.netUpdate = true;
				}
			}

			// Set the correct rotation of the projectile.
			projectile.rotation = (float)(Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + Math.PI);

			// Animate the projectile.
			if (projectile.frameCounter++ >= 3)
			{
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				projectile.frameCounter = 0;
			}
			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.ai[0] > 1)
			{
				if (projectile.velocity.X != oldVelocity.X)
					projectile.velocity.X = -oldVelocity.X;
				if (projectile.velocity.Y != oldVelocity.Y)
					projectile.velocity.Y = -oldVelocity.Y;

				projectile.ai[0]--;
			}
			else
			{
				projectile.ai[1] = 1;
				projectile.netUpdate = true;
			}
			return (false);
		}
	}
}
