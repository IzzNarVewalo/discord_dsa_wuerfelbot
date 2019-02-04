using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using _04_dsa.Services;
using _04_dsa.Modules;
using System.Linq;

namespace _04_dsa
{
    // This is a minimal example of using Discord.Net's command
    // framework - by no means does it show everything the framework
    // is capable of.
    //
    // You can find samples of using the command framework:
    // - Here, under the 02_commands_framework sample
    // - https://github.com/foxbot/DiscordBotBase - a bare-bones bot template
    // - https://github.com/foxbot/patek - a more feature-filled bot, utilizing more aspects of the library
    class Program
    {

        private DiscordSocketClient _client;
        // There is no need to implement IDisposable like before as we are
        // using dependency injection, which handles calling Dispose for us.
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            using (var services = ConfigureServices())
            {
                Helper.LoadConfig();
                _client = services.GetRequiredService<DiscordSocketClient>();

                _client.Log += LogAsync;
                _client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data, and never hard-coded.
                await _client.LoginAsync(TokenType.Bot, Helper.DiscordToken);//Environment.GetEnvironmentVariable("token"));
                await _client.StartAsync();
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .AddSingleton<DBService>()
                .AddSingleton<Helden_software_API>()
                .AddSingleton<DsaModule>()
                .AddSingleton<PublicModule>()
                .BuildServiceProvider();
        }
    }
}
