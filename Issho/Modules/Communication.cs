using Discord;
using Discord.Commands;
using DiscordUtils;
using System.Threading.Tasks;

namespace Issho.Modules
{
    public class Communication : ModuleBase
    {
        [Command("Info")]
        public async Task Info()
        {
            await ReplyAsync(embed: Utils.GetBotInfo(Program.StartTime, "Issho", Program.Client.CurrentUser));
        }

        [Command("Help")]
        public async Task Help()
        {
            await ReplyAsync(embed: new EmbedBuilder
            {
                Title = "Help",
                Description = "**Play [gamename]**: Play a game, you must join a vocal channel first. Give no argument to have the list of all games\n" +
                    "**Help**: Display this help\n" +
                    "**Info**: Display basic information about this bot",
                Color = Color.Purple
            }.Build());
        }
    }
}
