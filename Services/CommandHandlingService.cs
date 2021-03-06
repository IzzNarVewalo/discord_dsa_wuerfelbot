using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace _04_dsa.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _commands.Log += LogAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync(ServiceProvider services)
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            if (!message.HasCharPrefix('!', ref argPos))
                return;

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services); // we will handle the result in CommandExecutedAsync
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
            {
                //await context.Channel.SendMessageAsync($"error: {result.ToString()}");
                return;

            }

            // the command was succesful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                await LogAsync(new LogMessage(LogSeverity.Info, context.User.Username, context.Message.ToString()));
                return;
            }

            // self handle some errors
            if (!result.Error.Equals("BadArgCount"))
            {
                var eb = new EmbedBuilder();
                var embed = eb.AddField("Fehler", "Die Parameter sind nicht richtig eingegeben.\n" +
                    "**!help " + command.Value.Name + "** für weitere Informationen verwenden.")
                    .WithColor(Color.Red)
                    .Build();
                await context.Channel.SendMessageAsync(null, false, embed);
                return;
            }

            // the command failed, let's notify the user that something happened.
            if (!command.Value.Name.Equals("cheat"))
                await context.Channel.SendMessageAsync($"error: {result.ToString()}");
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}
