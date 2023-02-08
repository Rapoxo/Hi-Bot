using Discord.Interactions;
using Discord;
using Newtonsoft.Json;
using YamlDotNet.Serialization.NamingConventions;

namespace Hi_bot 
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {


        private async Task<string> SearchGif(string query)
        {
            
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            // Exportar para um outro arquivo
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var config = deserializer.Deserialize<Configuration>(File.ReadAllText("config.yaml"));


            string url = $"https://api.giphy.com/v1/gifs/search?api_key={config.GiphyToken}&q={query}&limit=25&offset=0&rating=g&lang=en";

            var client = new HttpClient();
            var apiRes = await client.GetStringAsync(url);

            if (apiRes == null)
            {
                throw new Exception("Erro no request da api");
            };
            dynamic json = JsonConvert.DeserializeObject(apiRes);
            string gifUrl = $"https://i.giphy.com/media/{json.data[0].id}/giphy.webp";

            return gifUrl;


        }

        [SlashCommand("hi", "esse e bom")]
        public async Task HandleHiCommand(string texto, string query)
        {
            try
            {
                string gifUrl = await SearchGif(query);

                await RespondAsync($"OMG [{texto}]({gifUrl}) HI!!!");
            } 
            catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }

        }

    }
}
