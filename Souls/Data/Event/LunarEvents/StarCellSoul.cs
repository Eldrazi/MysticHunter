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
	public class StarCellSoul : PostHMSoul, IEventSoul
	{
		public override short soulNPC => NPCID.StardustCellBig;
		public override string soulDescription => "Summon a splitting Star Cell.";

		public override short cooldown => 600;

		public override SoulType soulType => SoulType.Red;

		public override short ManaCost(Player p, short stack) => 5;
		public override bool SoulUpdate(Player p, short stack)
		{
			int damage = 150;
			int modifier = 1;

			if (stack >= 5)
			{
				damage += 20;
				modifier++;
			}
			if (stack >= 9)
			{
				damage += 30;
				modifier += 2;
			}

			Vector2 velocity = Vector2.Normalize(Main.MouseWorld - p.Center) * 6;
			Projectile.NewProjectile(p.Center, velocity, ModContent.ProjectileType<StarCellSoul_Proj>(), damage, 0.5f, p.whoAmI, modifier);

			return (true);
		}
	}

	public class StarCellSoul_Proj : ModProjectile
	{
		public override string Texture => "Terraria/NPC_" + NPCID.StardustCellBig;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Cell");

			Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.StardustCellBig];
		}
		public override void SetDefaults()
		{
			projectile.width = projectile.height = 22;

			projectile.timeLeft = 300;

			projectile.friendly = true;
			projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			if (++projectile.ai[1] >= 20)
			{
				projectile.velocity *= 0.98f;
			}

			projectile.scale = (projectile.ai[0] + 1) * 0.5f;

			// Animation.
			if (++projectile.frameCounter >= 10)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
			}

			return (false);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.ai[0]-- > 0)
			{
				for (int i = 0; i < 3; ++i)
				{
					Vector2 velocity = projectile.velocity.RotatedBy(MathHelper.PiOver4 * (i - 1)) * 0.75f;

					Projectile.NewProjectile(projectile.Center, velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0]);
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				Dust d = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0, 0, 100, default, 2f)];
				d.noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
			=> this.DrawAroundOrigin(spriteBatch, lightColor);
	}
}
