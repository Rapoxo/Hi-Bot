
using System.Reflection.Metadata.Ecma335;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Hi_bot
{
    public class Configuration
    {
        public string DiscordToken { get; set; }
        public string GiphyToken { get; set;}
    }

}
