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
            await ReplyAsync("", false, Utils.GetBotInfo(Program.StartTime, "Issho", Program.Client.CurrentUser));
        }
    }
}
