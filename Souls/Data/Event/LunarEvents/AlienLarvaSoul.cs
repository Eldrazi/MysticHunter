#region Using directives

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MysticHunter.Souls.Framework;
using MysticHunter.Common.Extensions;

#endregion

namespace MysticHunter.Souls.Data.Event.LunarEvents
{
	public class AlienLarvaSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.VortexLarva;
		public override string soulDescription => "Summons a friendly Alien Larva.";

		public override short cooldown => 60;

		public override SoulType soulType => SoulType.Blue;

		public override short ManaCost(Player p, short stack) => (short)(15 + (stack >= 9 ? 10 : 0));
		public override bool SoulUpdate(Player p, short stack)
		{
			int type1 = ModContent.ProjectileType<AlienLarvaSoul_Proj_Larva>();
			int type2 = ModContent.ProjectileType<AlienLarvaSoul_Proj_Hornet>();
			int type3 = ModContent.ProjectileType<AlienLarvaSoul_Proj_Queen>();

			for (int i = 0; i < Main.maxProjectiles; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == p.whoAmI &&
					(Main.projectile[i].type == type1 || Main.projectile[i].type == type2 || Main.projectile[i].type == type3))
				{
					Main.projectile[i].Kill();
					break;
				}
			}

			int damage = 100;
			int modifier = 0;

			if (stack >= 5)
			{
				damage += 50;
				modifier++;
			}
			if (stack >= 9)
			{
				damage += 50;
				modifier++;
			}

			if (modifier == 0)
			{
				Projectile.NewProjectile(p.Center, default, type1, damage, 0.5f, p.whoAmI);
			}
			else if (modifier == 1)
			{
				Projectile.NewProjectile(p.Center, default, type2, damage, 0.5f, p.whoAmI);
			}
			else
			{
				Projectile.NewProjectile(p.Center, default, type3, damage, 0.5f, p.whoAmI);
			}

			return (true);
		}
	}

	public class AlienLarvaSoul_Proj_Larva : ModProjectile
	{
		public override string Texture => "Terraria/NPC_"+ NPCID.VortexLarva;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alien Larva");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.VortexLarva];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 24;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 1f;

			projectile.aiStyle = 26;
			aiType = ProjectileID.BabySlime;

			projectile.minion = true;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.VortexLarva)
			{
				projectile.timeLeft = 2;
			}
			return (true);
		}

		public override void PostAI()
		{
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
				projectile.frame = 0;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
			=> false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}

	public class AlienLarvaSoul_Proj_Hornet : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.VortexHornet;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alien Hornet");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.VortexHornet];
		}
		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 28;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 1f;

			projectile.aiStyle = 26;
			aiType = ProjectileID.BabySlime;

			projectile.minion = true;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.VortexLarva)
			{
				projectile.timeLeft = 2;
			}
			return (true);
		}

		public override void PostAI()
		{
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
				projectile.frame = 0;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
			=> false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}

	public class AlienLarvaSoul_Proj_Queen : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.VortexHornetQueen;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alien Queen");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.VortexHornetQueen];
		}
		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 40;
			drawOriginOffsetY = 8;

			projectile.timeLeft *= 5;
			projectile.penetrate = -1;
			projectile.minionSlots = 1f;

			projectile.aiStyle = 26;
			aiType = ProjectileID.BabySlime;

			projectile.minion = true;
			projectile.netImportant = true;
		}

		public override bool PreAI()
		{
			Player owner = Main.player[projectile.owner];
			SoulPlayer sp = owner.GetModPlayer<SoulPlayer>();

			if (owner.active && !owner.dead && sp.BlueSoulNet.soulNPC == NPCID.VortexLarva)
			{
				projectile.timeLeft = 2;
			}
			return (true);
		}

		public override void PostAI()
		{
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
				projectile.frame = 0;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
			=> false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
