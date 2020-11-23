using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Bosses
{
	public class TheTwinsSoul : PostHMSoul
	{
		public override short soulNPC => NPCID.Spazmatism;
		public override string soulDescription => "Summon Retinazer and Spazmatism as minions.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Destroy any pre-existing projectile.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI &&
					(Main.projectile[i].type == ProjectileType<TheTwinsSoulRetinazerProj>() || Main.projectile[i].type == ProjectileType<TheTwinsSoulSpazmatismProj>()))
					Main.projectile[i].Kill();

			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<TheTwinsSoulRetinazerProj>(), 100 + 5 * stack, 0f, p.whoAmI, 0, 255);
			Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<TheTwinsSoulSpazmatismProj>(), 80 + 5 * stack, 0f, p.whoAmI, 0, 255);
			return (true);
		}

		public override string SoulNPCName()
			=> "The Twins";

		public override short[] GetAdditionalTypes()
			=> new short[] { NPCID.Retinazer };
	}

	public class TheTwinsSoulProj : ModProjectile
	{
		public override bool Autoload(ref string name)
			=> this.GetType().IsSubclassOf(typeof(TheTwinsSoulProj));

		private const float maxTargetDistance = 540;
		private const float maxTargetLosingDistance = 640;

		private int Target
		{
			get { return (int)projectile.ai[1]; }
			set { projectile.ai[1] = value; }
		}
		private bool HasTarget => Target != 255;

		protected virtual int SpawnProjectileID { get; }
		protected virtual float DefaultDesiredRot { get; }

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 18;

			projectile.scale = .2f;
			projectile.timeLeft *= 5;
			projectile.minionSlots = 0f;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];

			if (owner.active && !owner.dead && owner.GetModPlayer<SoulPlayer>().activeSouls[(int)SoulType.Blue].soulNPC == NPCID.Spazmatism)
				projectile.timeLeft = 2;

			if (projectile.localAI[0] == 0)
			{
				projectile.localAI[0] = 1;
				projectile.rotation = DefaultDesiredRot;
			}

			float desiredRotation = DefaultDesiredRot;

			if (!HasTarget)
			{
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];
						if (npc.CanBeChasedBy() && CanTarget(npc) && projectile.Distance(npc.Center) <= maxTargetDistance)
						{
							Target = i;
							projectile.netUpdate = true;
							break;
						}
					}
				}
			}
			else
			{
				NPC npc = Main.npc[Target];
				if (!npc.CanBeChasedBy() || !CanTarget(npc) || projectile.Distance(npc.Center) > maxTargetLosingDistance)
				{
					Target = 255;
					projectile.netUpdate = true;
				}
				else
				{
					if (projectile.owner == Main.myPlayer && projectile.ai[0]++ >= 90)
					{
						Projectile newProj = Main.projectile[Projectile.NewProjectile(projectile.Center, projectile.rotation.ToRotationVector2() * 8,
							SpawnProjectileID, projectile.damage, .1f, owner.whoAmI)];
						newProj.timeLeft = 300;
						newProj.friendly = true;
						newProj.hostile = false;
						newProj.netUpdate = true;

						projectile.ai[0] = 0;
					}
				}

				desiredRotation = (npc.Center - owner.Center).ToRotation();
				if (DefaultDesiredRot != 0 && desiredRotation < 0)
					desiredRotation = MathHelper.Pi + (MathHelper.PiOver2 - (Math.Abs(desiredRotation) - MathHelper.PiOver2));
			}

			// Position the projectile based on the desired rotation.
			projectile.rotation = MathHelper.Lerp(projectile.rotation, desiredRotation, .05f);
			projectile.Center = owner.Center + new Vector2((float)Math.Cos(projectile.rotation), (float)Math.Sin(projectile.rotation)) * 28;

			// Animation.
			if (projectile.frameCounter++ >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}
			if (projectile.frame < 3)
				projectile.frame = 3;

			return (false);
		}

		public override bool CanDamage() => false;

		protected virtual bool CanTarget(NPC npc) => true;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			Rectangle rect = tex.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rect, lightColor, projectile.rotation - MathHelper.PiOver2, rect.Size() / 2, projectile.scale, SpriteEffects.None, 0f);
			return (false);
		}
	}

	public sealed class TheTwinsSoulRetinazerProj : TheTwinsSoulProj
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Retinazer;

		protected override int SpawnProjectileID => ProjectileID.EyeLaser;
		protected override float DefaultDesiredRot => 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Retinazer");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.Retinazer];
		}

		protected override bool CanTarget(NPC npc)
			=> npc.Center.X > Main.player[projectile.owner].Center.X;
	}

	public sealed class TheTwinsSoulSpazmatismProj : TheTwinsSoulProj
	{
		public override string Texture => "Terraria/NPC_" + NPCID.Spazmatism;

		protected override int SpawnProjectileID => ProjectileID.EyeFire;
		protected override float DefaultDesiredRot => MathHelper.Pi;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spazmatism");
			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.Spazmatism];
		}

		protected override bool CanTarget(NPC npc)
			=> npc.Center.X < Main.player[projectile.owner].Center.X;
	}
}
