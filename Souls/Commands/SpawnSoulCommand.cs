using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using MysticHunter.Souls.Items;

namespace MysticHunter.Souls.Commands
{
	public class SpawnSoulCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;

		public override string Command => "spawnsoul";

		public override string Usage
			=> "/spawnsoul type [stack]";

		public override string Description
			=> "Spawns N number of souls of the specified type";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length < 1)
				throw new UsageException("Must provide at least one argument.");
			if (!int.TryParse(args[0], out int type))
				throw new UsageException(args[0] + " is not an integer.");
			if (type > Main.maxNPCTypes)
				throw new UsageException(args[0] + " is not a valid NPC type.");

			int stack;
			if (args.Length > 1)
			{
				if (!int.TryParse(args[1], out stack))
					stack = 1;
			}
			else
				stack = 1;

			for (int i = 0; i < stack; ++i)
			{
				int item = Item.NewItem(Main.LocalPlayer.Center, ItemType<BasicSoulItem>(), 1, true);
				if (Main.item[item].modItem is BasicSoulItem soul)
				{
					soul.soulNPC = (short)type;
				}
			}
		}
	}
}
