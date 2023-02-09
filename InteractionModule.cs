using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using Discord.Interactions;
using Newtonsoft.Json;
using YamlDotNet.Serialization.NamingConventions;
namespace Hi_bot 
{
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {



        private Bitmap WriteText(string text)
        {
            Bitmap image = (Bitmap)Image.FromFile("template.jpg");
            Graphics graphics = Graphics.FromImage(image);
            Font font = new Font("Arial", 38);
            Rectangle textContainer = new Rectangle(370, 510, 256, 128);
            graphics.FillRectangle(Brushes.Transparent, textContainer);
            textContainer.Inflate(-10, -10);
            graphics.DrawString(text, font, Brushes.Black, textContainer);

            return image;
            
        }

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
                var image = WriteText(texto);

                MemoryStream imageStream = new MemoryStream();
                image.Save(imageStream, ImageFormat.Gif);

                await RespondWithFileAsync(imageStream, "hii.jpg");
            } 
            catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }

        }

        [SlashCommand("template", "Responde com a imagem de template.")]
        public async Task HandleImageCommand(string text)
        { 
            await RespondWithFileAsync("template.jpg");
            
        }

    }
}
