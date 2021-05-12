using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordUtils;
using Issho.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        public static HttpClient Http { get; } = new();
        public static string Token { private set; get; }

        public static Dictionary<ulong, DateTime> WaitTimes { private set; get; } = new();

        private Program()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
            });
            Client.Log += Utils.Log;
            commands.Log += LogErrorAsync;
        }

        private async Task MainAsync()
        {
            Client.MessageReceived += HandleCommandAsync;

            await commands.AddModuleAsync<Communication>(null);
            await commands.AddModuleAsync<Modules.Game>(null);

            if (!File.Exists("Keys/token.txt"))
                throw new FileNotFoundException("Missing token.txt in Keys/");
            Token = File.ReadAllText("Keys/token.txt");
            await Client.LoginAsync(TokenType.Bot, Token);
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
                var result = await commands.ExecuteAsync(context, pos, null);
                if (!result.IsSuccess)
                {
                    var error = result.Error.Value;
                    if (error == CommandError.UnmetPrecondition || error == CommandError.BadArgCount || error == CommandError.ParseFailed)
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        public static async Task LogErrorAsync(LogMessage msg)
        {
            await Utils.Log(msg);
            CommandException ce = msg.Exception as CommandException;
            if (ce != null)
            {
                await ce.Context.Channel.SendMessageAsync("", false, new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = msg.Exception.InnerException.GetType().ToString(),
                    Description = "An unexpected error occured"
                }.Build());
            }
        }
    }
}
