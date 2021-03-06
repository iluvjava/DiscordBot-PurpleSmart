﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Utilities;
using Modules;

namespace Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;


        /// <summary>
        /// This method assemble all the components. 
        /// This is where all the depedency injections
        /// are working. 
        /// </summary>
        /// <param name="services">
        /// 
        /// </param>
        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }



        /// <summary>
        /// This is the method that reads all the rawmessage that presents at the discord 
        /// channels. 
        /// <param>
        /// The method analyze the rawMessage and decide what to do, a pretty important method. 
        /// This is also the place when the bot first meet the context of the conversation. 
        /// </param>
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <returns></returns>
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {

            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;

           

            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            //if (!message.HasStringPrefix("!", ref argPos)) return;
            var context = new SocketCommandContext(_discord, message);
            RawMessageContext rmcb = new RawMessageContext(rawMessage,context);
            MessageContext mc = new MessageContext(context);

            string CommandPrefix = Manager.GetServerCommandPrefix
                (
                    mc
                );

            //if bot is mentioned.
            bool BotIsMentioned = rmcb.BotIsMentioned();
            if (BotIsMentioned)
            {
                Utilities.Stuff.ConsoleLog("Bot is mentioned.");
                rmcb.sendMessageToThisChannel("Use"+CommandPrefix+"help for more info.");
            } 
            
            
            // This value holds the offset where the prefix ends
            var argPos = CommandPrefix.Length-1;// 0;


            if (!message.HasStringPrefix(CommandPrefix, ref argPos)) return;
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.

            await _commands.ExecuteAsync(context, argPos, _services);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }



        /// <summary>
        /// This method execute the command, and the parameters passed in are the context we can 
        /// use to choose certain way we can execute the commands. 
        /// </summary>
        /// <param name="command">
        /// 
        /// </param>
        /// <param name="context">
        /// 
        /// </param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task CommandExecutedAsync
        (Optional<CommandInfo> command, ICommandContext context, IResult result)
        {

            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                return;
            }

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync();//$"error: {result}");
        }
    }
}
