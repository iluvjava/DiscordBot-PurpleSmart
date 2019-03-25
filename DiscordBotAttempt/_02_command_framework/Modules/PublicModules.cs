using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotAttempt._02_command_framework.Modules;
using MathyStuff;
using Services;
using Utilities;


namespace Modules
{

    /// 
    /// <summary>
    /// A class that has concrete implementation for each 
    /// of the command.
    /// Modules must be public and inherit from an IModuleBase
    /// This class has concrete implementation for specific 
    /// <para>
    /// The discord infrastrcture uses dependency injection, hence
    /// method attributes are serves important role when it uses 
    /// c sharp reflection. 
    /// </para>
    /// 
    /// <para>
    /// Please read ModlulesBass class to get more ideas about the context of this class. 
    /// </para>
    /// </summary>
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }


        /// <summary>
        /// This is an example of a method that does hase any input parameter,
        /// it means the bot is going to do something when the 
        /// commands appeared at the discord channel. 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [Command("help")]
        [Alias("Help", "HELP")]
        public async Task PingAsync()
        {
            string formattedreply = CommandConstant.HELPSTRING;
            Random rd = new Random();
            Interpretor temp = new Interpretor(CommandConstant.EXAMPLES[rd.Next(2)]);
            formattedreply += "Your Input: \""+temp.inputexpression+"\"+\n";
            formattedreply += "The Output will be: " + temp.outputresult;
            await ReplyAsync(formattedreply);
        }

        //[Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        //[Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }

        /// <summary>
        ///   Ban a user;
        ///   This method demonstrated how to use method attributes 
        ///   to configure command, and how input parameters can 
        ///   be setted for reflection. 
        /// </summary>
      
        //[Command("ban")]
        [RequireContext(ContextType.Guild)]
        // make sure the user invoking the command can ban
        [RequireUserPermission(GuildPermission.BanMembers)]
        // make sure the bot itself can ban
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            await user.Guild.AddBanAsync(user, reason: reason);
            await ReplyAsync("ok!");
        }

        
        
        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        //[Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

      
        
        /// 
        /// <summary>
        /// <para>
        /// Method has been tested casually. 
        /// </para>
        /// 'params' will parse space-separated elements into a list
        /// This method split the input of uesr separated by space
        /// into a String objects. 
        /// <para>
        /// The command itself will not be in the objects. 
        /// </para>
        /// </summary>
        [Command("math")]
        public Task MathAsync(params string[] objects)
        {
            Console.WriteLine("MathAsync Triggered, Context Object: " + this.Context.ToString());

            ICommandHandles A_MathCommand = new TheMathCommand(new MessageContextBridge(this.Context , objects));

            return ReplyAsync(A_MathCommand.getReply());
        }

        /// <summary>
        /// Setting a custom ErrorMessage property will help clarify the precondition error, 
        /// This method demonstrates how to make a context specific commands 
        /// configuration. 
        /// </summary>
        
        [Command("guild_only")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public Task GuildOnlyCommand()
            => ReplyAsync("Nothing to see here!");
    }


    /// <summary>
    /// May be an enum class is better? 
    /// </summary>
    public class CommandConstant
    {

        public const string HELPSTRING =
            "Use !math command to solve integer arithmatic.\n"
            +
            "Here is an example: "
            ;
        public static readonly string[] EXAMPLES ={"2+9/5-5*(334/45)","333/452+3/(5*88-6)"};
    }
}
