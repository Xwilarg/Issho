using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private Dictionary<string, string> games = new Dictionary<string, string>
        {
            { "Poker Night", "755827207812677713" },
            { "Betrayal.io", "773336526917861400" },
            { "YouTube Together", "755600276941176913" },
            { "Fishington.io", "814288819477020702" },
            { "Chess", "832012586023256104" },
            { "CG 3 Dev", "832012682520428625" },
            { "Game 3", "832012730599735326" },
            { "Game 4", "832012774040141894" },
            { "Game 5", "832012815819604009" },
            { "Game 6", "832012854282158180" },
            { "Game 7", "832012894068801636" },
            { "Game 8", "832012938398400562" },
            { "Game 9", "832013003968348200" },
            { "Game 10", "832013108234289153" },
            { "Game 11", "832025061657280566" },
            { "Game 12", "832025114077298718" },
            { "Game 13", "832025144389533716" },
            { "Game 14", "832025179659960360" },
            { "Game 15", "832025219526033439" },
            { "Game 16", "832025249335738428" },
            { "Game 17", "832025333930524692" },
            { "Game 18", "832025385159622656" },
            { "Game 19", "832025431280320532" },
            { "Game 20", "832025470685937707" },
            { "Game 21", "832025799590281238" },
            { "Game 22", "832025857525678142" },
            { "Game 23", "832025886030168105" },
            { "Game 24", "832025928938946590" },
            { "Game 25", "832025993019260929" },
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
                    Description = "Available games:\n" + string.Join(", ", games.Keys),
                    Color = Color.Purple
                }.Build());
            }
            else
            {
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
                System.Console.WriteLine(JsonConvert.DeserializeObject<JObject>(json));
                await ReplyAsync("https://discord.gg/" + JsonConvert.DeserializeObject<JObject>(json)["code"]);
            }
        }
    }
}
