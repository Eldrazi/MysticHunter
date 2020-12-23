#region Using directives

using Terraria;
using Terraria.ModLoader;

#endregion

namespace MysticHunter.Souls.Commands
{
	public class RemoveSoulCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;

		public override string Command => "removesoul";

		public override string Usage
			=> "/removesoul type [stack]";

		public override string Description
			=> "Spawns N number of souls of the specified type";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length < 1)
				throw new UsageException("Must provide at least one argument.");
			if (!short.TryParse(args[0], out short type))
				throw new UsageException(args[0] + " is not an short.");
			if (type > Main.maxNPCTypes)
				throw new UsageException(args[0] + " is not a valid NPC type.");

			byte stack;
			if (args.Length > 1)
			{
				if (!byte.TryParse(args[1], out stack))
					stack = 1;
			}
			else
				stack = 1;

			SoulPlayer sp = Main.LocalPlayer.GetModPlayer<SoulPlayer>();

			if (sp.UnlockedSouls.ContainsKey(type))
			{
				if (sp.UnlockedSouls[type] < stack)
					stack = sp.UnlockedSouls[type];
				sp.UnlockedSouls[type] -= stack;
			}
		}
	}
}
