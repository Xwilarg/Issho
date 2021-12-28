using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Issho.Modules
{
    public class Game : ModuleBase
    {
        private Dictionary<string, string> games = new()
        {
            { "Poker Night", "755827207812677713" },
            { "Betrayal.io", "773336526917861400" },
            { "Fishington.io", "814288819477020702" },
            { "Chess In The Park", "832012774040141894" },
            { "Checkers In The Park", "832013003968348200" },
            { "Watch Together", "880218394199220334" },
            { "Letter Tile", "879863686565621790" },
            { "Word Snacks", "879863976006127627" },
            { "Doodle Crew", "878067389634314250" },
            { "SpellCast", "852509694341283871" },
            { "SpellCast Staging", "893449443918086174" }
        };

        [Command("Play")]
        public async Task Info(params string[] gameName)
        {

            string name = gameName == null ? null : string.Join(" ", gameName).ToUpperInvariant().Trim();
            if (string.IsNullOrWhiteSpace(name) || !games.Keys.Any(x => x.ToUpperInvariant() == name))
            {

                await ReplyAsync(embed: new EmbedBuilder
                {
                    Title = "Usage: Play [Game Name]",
                    Description = "**Available games:**\n" + string.Join(", ", games.Keys),
                    Color = Color.Purple
                }.Build());
                return;
            }
            if (Program.WaitTimes.ContainsKey(Context.Guild.Id))
            {
                var time = DateTime.Now - Program.WaitTimes[Context.Guild.Id];
                if (time.TotalSeconds < 10)
                {
                    await ReplyAsync("You must wait " + (10 - time.TotalSeconds).ToString("0.00") + " more seconds to do the command again in this guild");
                    return;
                }
            }
            if (Context.User is not IGuildUser user || user.VoiceChannel == null)
            {
                await ReplyAsync("You must be in a voice channel to use this command.");
                return;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/v9/channels/" + user.VoiceChannel.Id + "/invites");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bot", Program.Token);
            request.Content = new StringContent("{\"target_type\":2,\"target_application_id\":\"" + games[games.Keys.First(x => x.ToUpperInvariant() == name)] + "\"}", Encoding.UTF8, "application/json");

            var answer = await Program.Http.SendAsync(request);
            var json = await answer.Content.ReadAsStringAsync();
            var code = JsonConvert.DeserializeObject<JObject>(json)["code"].Value<string>();
            if (code == "50013")
            {
                await ReplyAsync("I wasn't able to create an invite link, please make sure I have the permission for that");
            }
            else
            {
                await ReplyAsync("<https://discord.gg/" + code + ">");
            }
            if (Program.WaitTimes.ContainsKey(Context.Guild.Id))
            {
                Program.WaitTimes[Context.Guild.Id] = DateTime.Now;
            }
            else
            {
                Program.WaitTimes.Add(Context.Guild.Id, DateTime.Now);
            }
        }
    }
}
