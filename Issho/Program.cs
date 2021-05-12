using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordUtils;
using Issho.Modules;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Issho
{
    public class Program
    {
        public static async Task Main()
            => await new Program().MainAsync();

        public static DiscordSocketClient Client { private set;  get; }
        private readonly CommandService commands = new();
        public static DateTime StartTime { private set; get; }

        private Program()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
            });
            Client.Log += Utils.Log;
            commands.Log += Utils.LogErrorAsync;
        }

        private async Task MainAsync()
        {
            Client.MessageReceived += HandleCommandAsync;

            await commands.AddModuleAsync<Communication>(null);

            if (!File.Exists("Keys/token.txt"))
                throw new FileNotFoundException("Missing token.txt in Keys/");
            await Client.LoginAsync(TokenType.Bot, File.ReadAllText("Keys/token.txt"));
            StartTime = DateTime.Now;
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage msg = arg as SocketUserMessage;
            if (msg == null || arg.Author.IsBot) return;
            int pos = 0;
            if (msg.HasMentionPrefix(Client.CurrentUser, ref pos) || msg.HasStringPrefix("i.", ref pos))
            {
                SocketCommandContext context = new SocketCommandContext(Client, msg);
                await commands.ExecuteAsync(context, pos, null);
            }
        }
    }
}
