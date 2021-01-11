#region Using directives

using System;
using System.IO;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

#endregion

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class CrimeraSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.Crimera;
		public override string soulDescription => "Summons a friendly crimera.";

		public override short cooldown => 300;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(10 + stack);
		public override bool SoulUpdate(Player p, short stack)
		{
			// Kill any active Eater of Souls projectiles.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<CrimeraSoulProj>())
					Main.projectile[i].Kill();

			Projectile.NewProjectile(p.Center, Vector2.Zero, ModContent.ProjectileType<CrimeraSoulProj>(), 20 + stack, .1f + .02f * stack, p.whoAmI);
			return (true);
		}

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.LittleCrimera, NPCID.BigCrimera };
	}

	public class CrimeraSoulProj : ModProjectile
	{
		private readonly float speed = 5;
		private readonly float acceleration = .05f;
		private readonly int targetingDistance = 320;
		private readonly int maxTargetingDistance = 480;

		private int targetIndex = 0;

		public override string Texture => "Terraria/Images/NPC_" + NPCID.Crimera;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crimera");
			Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 32;

			projectile.scale = .6f;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.friendly = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			// Check to see if the NPC should still be alive.
			if (owner.active && !owner.dead && sp.RedSoulNet.soulNPC == NPCID.Crimera)
				projectile.timeLeft = 2;

			bool hasTarget = true;
			NPC target = Main.npc[targetIndex];
			if (!target.CanBeChasedBy() || Vector2.Distance(projectile.Center, target.Center) > maxTargetingDistance)
			{
				TargetClosestNPC();
				target = Main.npc[targetIndex];
				if (targetIndex == 0)
					hasTarget = false;
			}

			Vector2 targetPosition = owner.Center;
			if (hasTarget)
				targetPosition = target.Center;

			// Calculate position relative to target and move accordingly.
			Vector2 direction = ((targetPosition / 8) * 8) - ((projectile.Center / 8) * 8);
			float length = direction.Length();

			// direction vector normalization with speed parameter.
			if (length != 0)
				direction *= (speed / length);
			
			// If the projectile does not have a target, make sure it can follow the player properly.
			if (!hasTarget && !Collision.CanHitLine(projectile.position, projectile.width, projectile.height, owner.position, owner.width, owner.height))
				projectile.tileCollide = false;
			// If it *does* have a target but is set to not collide with tiles, takes proper measurements.
			else if (projectile.tileCollide == false)
			{
				if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					projectile.tileCollide = true;
			}

			if (length > 100f)
			{
				if (projectile.ai[0]++ > 0f)
					projectile.velocity.Y += .023f;
				else
					projectile.velocity.Y -= .023f;
				if (projectile.ai[0] < -100f || projectile.ai[0] > 100f)
					projectile.velocity.X += .023f;
				else
					projectile.velocity.X -= .023f;

				if (projectile.ai[0] > 200f)
					projectile.ai[0] = -200f;
			}
			if (length < 150f)
			{
				projectile.velocity.X += direction.X * .007f;
				projectile.velocity.Y += direction.Y * .007f;
			}

			// Set velocity accordingly.
			if (projectile.velocity.X < direction.X)
				projectile.velocity.X += acceleration;
			else if (projectile.velocity.X > direction.X)
				projectile.velocity.X -= acceleration;

			if (projectile.velocity.Y < direction.Y)
				projectile.velocity.Y += acceleration;
			else if (projectile.velocity.Y > direction.Y)
				projectile.velocity.Y -= acceleration;

			// Set the correct rotation for the projectile.
			projectile.rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;

			// Netupdate if the velocity has changed 'dramatically'.
			if ((projectile.velocity.X > 0f && projectile.oldVelocity.X < 0f) || (projectile.velocity.X < 0f && projectile.oldVelocity.X > 0f) || (projectile.velocity.Y > 0f && projectile.oldVelocity.Y < 0f) || (projectile.velocity.Y < 0f && projectile.oldVelocity.Y > 0f))
				projectile.netUpdate = true;

			// Spawn some fancy particles.
			if (Main.rand.Next(20) == 0)
			{
				Dust d = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + projectile.height * 0.25f), 
					projectile.width, (int)(projectile.height * 0.5f), 18, projectile.velocity.X, 2f, 75, default, projectile.scale)];
				d.velocity *= new Vector2(.5f, .1f);
			}

			// Animate the projectile.
			if (projectile.frameCounter++ >= 6)
			{
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
				projectile.frameCounter = 0;
			}

			return (false);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.netUpdate = true;
				projectile.velocity.X = oldVelocity.X * -.4f;

				if (projectile.direction == -1 && projectile.velocity.X > 0f && projectile.velocity.X < 2f)
					projectile.velocity.X = 2f;
				if (projectile.direction == 1 && projectile.velocity.X < 0f && projectile.velocity.X > -2f)
					projectile.velocity.X = -2f;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.netUpdate = true;
				projectile.velocity.Y = oldVelocity.Y * -.4f;

				if (projectile.velocity.Y > 0f && projectile.velocity.Y < 1.5f)
					projectile.velocity.Y = 2f;
				if (projectile.velocity.Y < 0f && projectile.velocity.Y > -1.5f)
					projectile.velocity.Y = -2f;
			}
			return (false);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 18, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		private void TargetClosestNPC()
		{
			for (int i = 0; i < Main.maxNPCs; ++i)
			{
				if (Main.npc[i].CanBeChasedBy() && Vector2.Distance(projectile.Center, Main.npc[i].Center) <= targetingDistance)
				{
					targetIndex = i;
					projectile.netUpdate = true;
					return;
				}
			}

			// No valid target found.
			targetIndex = 0;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 origin = new Vector2(texture.Width * .5f, (texture.Height / Main.projFrames[projectile.type]) * .5f);

			Rectangle frame = new Rectangle(0, (texture.Height / Main.projFrames[projectile.type]) * projectile.frame, texture.Width, texture.Height / Main.projFrames[projectile.type]);

			spriteBatch.Draw(texture, projectile.position + origin * projectile.scale - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0);

			return (false);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(this.targetIndex);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			this.targetIndex = reader.ReadInt32();
		}
	}
}
