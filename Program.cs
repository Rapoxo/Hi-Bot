using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YamlDotNet.Serialization.NamingConventions;

namespace Hi_bot
{
    public class Program
    {
        public static Task Main() => new Program().MainAsync();

        public async Task MainAsync()
        {

            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                services
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .AddSingleton<InteractionHandler>()
                )
                .Build();

            await RunAsync(host);

        }

        public async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var _client = provider.GetRequiredService<DiscordSocketClient>();
            var sCommands = provider.GetRequiredService<InteractionService>();
            await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

            

            _client.Log += async (msg) => { Console.WriteLine(msg.Message); };
            sCommands.Log +=  async (msg) => { Console.WriteLine(msg.Message); };

            _client.Ready += async () => 
            {
                await sCommands.RegisterCommandsGloballyAsync();
                Console.WriteLine("HI!!!!! DISCORD"); 
            };

            // Exportar para um outro arquivo
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var config = deserializer.Deserialize<Configuration>(File.ReadAllText("config.yaml"));


            await _client.LoginAsync(TokenType.Bot, config.DiscordToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

    }
}