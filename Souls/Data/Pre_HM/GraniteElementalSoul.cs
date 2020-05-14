using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Microsoft.Xna.Framework;

using MysticHunter.Souls.Framework;

namespace MysticHunter.Souls.Data.Pre_HM
{
	public class GraniteElementalSoul : PreHMSoul
	{
		public override short soulNPC => NPCID.GraniteFlyer;
		public override string soulDescription => "Summons a protective granite elemental.";

		public override short cooldown => 0;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => 30;
		public override bool SoulUpdate(Player p, short stack)
		{
			// Kill any active GraniteElementalSoulNPCs.
			for (int i = 0; i < Main.maxProjectiles; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI && Main.projectile[i].type == ProjectileType<GraniteElementalSoulProjectile>())
					Main.projectile[i].Kill();

			// Set some values depending on the stack amount.
			int amount = 1;
			if (stack >= 5)
				amount++;
			if (stack >= 9)
				amount++;

			// Spawn the new elemental(s).
			for (int i = 0; i < amount; ++i)
				Projectile.NewProjectile(p.Center, Vector2.Zero, ProjectileType<GraniteElementalSoulProjectile>(), 0, 0f, p.whoAmI, i, amount);

			return (true);
		}
	}

	/// <summary>
	/// projectile.ai[0] = Index of the elemental.
	/// projectile.ai[1] = Total amount of elementals.
	/// npc.ai[3] = Rotation/orbit regulator.
	/// </summary>
	public class GraniteElementalSoulProjectile : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.GraniteFlyer;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Elemental Shield");
			Main.projFrames[projectile.type] = 22;
		}
		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 46;

			projectile.scale = .8f;
			projectile.timeLeft *= 5;
			projectile.minionSlots = 0;

			projectile.minion = true;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;

			drawOffsetX = -22;
			drawOriginOffsetY = -14;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			// Check to see if the NPC should still be alive.
			if (owner.active && !owner.dead && sp.activeSouls[(int)SoulType.Blue].soulNPC == NPCID.GraniteFlyer)
				projectile.timeLeft = 2;

			if (projectile.localAI[1] == 0)
			{
				SpawnDust();
				projectile.localAI[1] = 1;
			}

			// Start calculating the correct position of the elemental.
			Vector2 newPos = owner.Center + new Vector2(0, 4);

			// We calculate the new position of the NPC based on its given index (npc.ai[1]) and a timer/counter which makes it rotate constantly (npc.ai[2]).
			newPos.X += (float)Math.Cos((MathHelper.TwoPi / projectile.ai[1]) * projectile.ai[0] + projectile.localAI[0]) * 50;
			newPos.Y += (float)Math.Sin((MathHelper.TwoPi / projectile.ai[1]) * projectile.ai[0] + projectile.localAI[0]) * 50;
			projectile.Center = newPos;
			projectile.localAI[0] += .035f;

			// Hit check.
			if (owner.whoAmI == Main.myPlayer)
			{
				for (int i = 0; i < Main.maxProjectiles; ++i)
					if (Main.projectile[i].active && Main.projectile[i].hostile && projectile.Hitbox.Intersects(Main.projectile[i].Hitbox))
						BlockProjectile(Main.projectile[i]);
			}

			// Animation.
			if (projectile.frameCounter++ >= 6)
			{
				projectile.frame = (projectile.frame + 1) % 12;
				projectile.frameCounter = 0;
			}
			return (false);
		}//SoundID.NPCHit7

		public override void Kill(int timeLeft)
		{
			SpawnDust();
		}

		private void SpawnDust()
		{
			for (int i = 0; i < 5; i++)
				Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Granite, projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 100);
		}

		private void BlockProjectile(Projectile proj)
		{
			// Do not despawn penetrating projectiles.
			if (proj.penetrate == -1)
				return;

			// Kill the projectile in question.
			proj.Kill();

			// Spawn extra dust flair around the killed projectile.
			for (int i = 0; i < 5; i++)
				Dust.NewDust(proj.position, proj.width, proj.height, DustID.Granite, proj.velocity.X * .2f, proj.velocity.Y * .2f, 100);
		}
	}
}
