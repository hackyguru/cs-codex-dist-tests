﻿using ArgsUniform;
using BiblioTech.Commands;
using BiblioTech.Rewards;
using Discord;
using Discord.WebSocket;
using DiscordRewards;
using Logging;

namespace BiblioTech
{
    public class Program
    {
        private DiscordSocketClient client = null!;

        public static Configuration Config { get; private set; } = null!;
        public static UserRepo UserRepo { get; } = new UserRepo();
        public static AdminChecker AdminChecker { get; private set; } = null!;
        public static IDiscordRoleDriver RoleDriver { get; set; } = null!;
        public static ILog Log { get; private set; } = null!;
        public static MarketAverage[] Averages { get; set; } = Array.Empty<MarketAverage>();

        public static Task Main(string[] args)
        {
            var uniformArgs = new ArgsUniform<Configuration>(PrintHelp, args);
            Config = uniformArgs.Parse();

            Log = new LogSplitter(
                new FileLog(Path.Combine(Config.LogPath, "discordbot")),
                new ConsoleLog()
            );

            EnsurePath(Config.DataPath);
            EnsurePath(Config.UserDataPath);
            EnsurePath(Config.EndpointsPath);

            return new Program().MainAsync(args);
        }

        public async Task MainAsync(string[] args)
        {
            Log.Log("Starting Codex Discord Bot...");
            client = new DiscordSocketClient();
            client.Log += ClientLog;

            var notifyCommand = new NotifyCommand();
            var associateCommand = new UserAssociateCommand(notifyCommand);
            var sprCommand = new SprCommand();
            var handler = new CommandHandler(client,
                new GetBalanceCommand(associateCommand), 
                new MintCommand(associateCommand),
                sprCommand,
                associateCommand,
                notifyCommand,
                new AdminCommand(sprCommand),
                new MarketCommand()
            );

            await client.LoginAsync(TokenType.Bot, Config.ApplicationToken);
            await client.StartAsync();
            AdminChecker = new AdminChecker();

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.ListenAnyIP(Config.RewardApiPort);
            });
            builder.Services.AddControllers();
            var app = builder.Build();
            app.MapControllers();

            Log.Log("Running...");
            await app.RunAsync();
            await Task.Delay(-1);
        }

        private static void PrintHelp()
        {
            Log.Log("BiblioTech - Codex Discord Bot");
        }

        private Task ClientLog(LogMessage msg)
        {
            Log.Log("DiscordClient: " + msg.ToString());
            return Task.CompletedTask;
        }

        private static void EnsurePath(string path)
        {
            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
        }
    }
}
