using System;
using System.IO;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MysticHunter.Souls.Data.Bosses
{
	public class LunaticCultistSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.CultistBoss;
		public override string soulDescription => "Summon friendly Phantasm Dragon.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer)
				{
					if (Main.projectile[i].modProjectile != null && Main.projectile[i].modProjectile is LunaticCultistSoulProj)
						Main.projectile[i].Kill();
				}
			}

			int currentProj = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ProjectileType<LunaticCultistSoulProj_Head>(), 100 + 5 * stack, 1, p.whoAmI);
			int previousProj = currentProj = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ProjectileType<LunaticCultistSoulProj_BodyWithArm>(), 100 + 5 * stack, 1, p.whoAmI, currentProj);
			currentProj = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ProjectileType<LunaticCultistSoulProj_BodyWithoutArm>(), 100 + 5 * stack, 1, p.whoAmI, currentProj);
			Main.projectile[previousProj].localAI[1] = currentProj;
			previousProj = currentProj;
			Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ProjectileType<LunaticCultistSoulProj_Tail>(), 100 + 5 * stack, 1, p.whoAmI, currentProj);
			Main.projectile[previousProj].localAI[1] = currentProj;
			return (true);
		}
	}

	internal class LunaticCultistSoulProj : ModProjectile
	{
		public override bool Autoload(ref string name)
			=> GetType().IsSubclassOf(typeof(LunaticCultistSoulProj));

		private const float DefaultScale = .5f;

		protected virtual bool IsHead => false;

		public override void SetStaticDefaults()
			=> DisplayName.SetDefault("Phantasm Dragon");
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 24;

			projectile.scale = DefaultScale;
			projectile.alpha = 255;
			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 0;

			projectile.hide = true;
			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().BlueSoulNet.soulNPC == NPCID.CultistBoss)
				projectile.timeLeft = 2;

			// Auto-update every 2 seconds.
			if ((int)Main.time % 120 == 0)
				projectile.netUpdate = true;

			if (this.IsHead) // Head AI.
			{
				int targetIndex = -1;
				Vector2 ownerPos = owner.Center;

				float maxTargetingDistance = 1000;
				float currentTargetingDistance = 700;

				if (projectile.Distance(ownerPos) > 2000)
				{
					projectile.netUpdate = true;
					projectile.Center = ownerPos;
				}

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					if (Main.npc[i].CanBeChasedBy(projectile) && owner.Distance(Main.npc[i].Center) < maxTargetingDistance)
					{
						float distanceToNPC = projectile.Distance(Main.npc[i].Center);
						if (distanceToNPC < currentTargetingDistance)
						{
							targetIndex = i;
							currentTargetingDistance = distanceToNPC;
						}
					}
				}

				if (targetIndex != -1)
				{
					NPC target = Main.npc[targetIndex];

					Vector2 direction = target.Center - projectile.Center;

					float speedModifer = .4f;
					if (direction.Length() < 600)
						speedModifer += .2f;
					if (direction.Length() < 300)
						speedModifer += .2f;

					if (direction.Length() > target.Size.Length() * .75f)
					{
						projectile.velocity += Vector2.Normalize(direction) * speedModifer * 1.5f;
						if (Vector2.Dot(projectile.velocity, direction) < .25f)
							projectile.velocity *= .8f;
					}
					if (projectile.velocity.Length() > 30)
						projectile.velocity = Vector2.Normalize(projectile.velocity) * 30;
				}
				else
				{
					float speedModifier = .2f;
					Vector2 direction = ownerPos - projectile.Center;

					if (direction.Length() < 200)
						speedModifier = .12f;
					if (direction.Length() < 140)
						speedModifier = .06f;

					if (direction.Length() > 100)
					{
						if (Math.Abs(direction.X) > 20)
							projectile.velocity.X = projectile.velocity.X + speedModifier * Math.Sign(direction.X);
						if (Math.Abs(direction.Y) > 10)
							projectile.velocity.Y = projectile.velocity.Y + speedModifier * Math.Sign(direction.Y);
					}
					else if (projectile.velocity.Length() > 2)
						projectile.velocity *= .96f;

					if (Math.Abs(projectile.velocity.Y) < 1)
						projectile.velocity.Y -= .1f;

					if (projectile.velocity.Length() > 15)
						projectile.velocity = Vector2.Normalize(projectile.velocity) * 15;
				}

				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
				int oldDirection = projectile.direction;
				projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0 ? 1 : -1);
				if (oldDirection != projectile.direction)
					projectile.netUpdate = true;

				float scaleModifier = MathHelper.Clamp(projectile.localAI[0], 0, 50);
				projectile.position = projectile.Center;
				projectile.scale = DefaultScale + scaleModifier * .01f;
				projectile.width = projectile.height = (int)(30 * projectile.scale);
				projectile.Center = projectile.position;

				if (projectile.alpha > 0)
				{
					projectile.alpha -= 42;
					if (projectile.alpha < 0)
						projectile.alpha = 0;
				}
			}
			else // Body part AI.
			{
				bool stayAlive = false;
				Vector2 otherCenter = Vector2.Zero;
				float otherRotation = 0;
				float scaleModifier = 0;

				if (projectile.ai[1] == 1)
				{
					projectile.ai[1] = 0;
					projectile.netUpdate = true;
				}

				int byUUID = Projectile.GetByUUID(projectile.owner, (int)projectile.ai[0]);
				if (byUUID >= 0 && Main.projectile[byUUID].active && Main.projectile[byUUID].modProjectile != null && Main.projectile[byUUID].modProjectile is LunaticCultistSoulProj)
				{
					Projectile other = Main.projectile[byUUID];

					stayAlive = true;
					otherCenter = other.Center;
					otherRotation = other.rotation;

					float scale = MathHelper.Clamp(other.scale, 0, 50f);
					scaleModifier = scale;
					projectile.alpha = other.alpha;
					other.localAI[0] = projectile.localAI[0] + 1;
					if (!(other.modProjectile as LunaticCultistSoulProj).IsHead)
						other.localAI[1] = projectile.whoAmI;
				}

				if (stayAlive)
				{
					projectile.velocity = Vector2.Zero;
					Vector2 directionToOther = otherCenter - projectile.Center;
					if (otherRotation != projectile.rotation)
					{
						float rot = MathHelper.WrapAngle(otherRotation - projectile.rotation);
						directionToOther = directionToOther.RotatedBy(rot * .1f);
					}

					projectile.rotation = directionToOther.ToRotation() + MathHelper.PiOver2;
					projectile.position = projectile.Center;
					projectile.scale = scaleModifier;
					projectile.width = projectile.height = (int)(30 * scaleModifier);
					projectile.Center = projectile.position;

					if (directionToOther != Vector2.Zero)
						projectile.Center = otherCenter - Vector2.Normalize(directionToOther) * 30 * scaleModifier;
					projectile.spriteDirection = (directionToOther.X > 0 ? 1 : -1);
				}
			}

			return (false);
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
			=> drawCacheProjsBehindProjectiles.Add(index);

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle rect = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rect, lightColor, projectile.rotation, rect.Size() / 2, projectile.scale,
				projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			return (false);
		}
	}

	internal sealed class LunaticCultistSoulProj_Head : LunaticCultistSoulProj
	{
		public override string Texture => "Terraria/NPC_" + NPCID.CultistDragonHead;

		protected override bool IsHead => true;

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.CultistDragonHead];
		}
	}
	internal sealed class LunaticCultistSoulProj_BodyWithArm : LunaticCultistSoulProj
	{
		public override string Texture => "Terraria/NPC_" + NPCID.CultistDragonBody1;
	}
	internal sealed class LunaticCultistSoulProj_BodyWithoutArm : LunaticCultistSoulProj
	{
		public override string Texture => "Terraria/NPC_" + NPCID.CultistDragonBody2;
	}
	internal sealed class LunaticCultistSoulProj_Tail : LunaticCultistSoulProj
	{
		public override string Texture => "Terraria/NPC_" + NPCID.CultistDragonTail;
	}
}
