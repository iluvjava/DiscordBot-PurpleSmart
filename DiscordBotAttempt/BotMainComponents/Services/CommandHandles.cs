
using Modules;
using MathyStuff;
using System;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using System.Text;

namespace Services
{

    /// <summary>
    /// There are so many different ways a command can get handled.
    /// and there are many different kinds of implementations, 
    /// so why not just defined the general ways that 
    /// we want it to have? 
    /// </summary>
    interface ICommandHandles
    {


        /// <summary>
        /// Maybe this commands needs some context to handle
        /// it, then this is the methods to implement. 
        /// </summary>
        /// <param name="mcb"></param>
        /// <returns></returns>
        string getReply();

        
        
        
    }

    #region math Command
    /// <summary>
    /// This is the math command we want to focus on. 
    /// </summary>
    /// 
    public class TheMathCommand : ICommandHandles
    {
        const string HELPTIPS = null; 

        protected MessageContext Context;

        public TheMathCommand(MessageContext arg)
        {
            this.Context = arg; 
        }


        public string getReply()
        {
            string[] WhatUserAsked = Context.getUserInput();
            string StringOutput = "";
            
            //the function that return the result we got in this function. 
            

            
            if (WhatUserAsked == null)
            {
                //Command cannot be run. 
                StringOutput = null;
                return StringOutput;
            };

            if (WhatUserAsked.Length == 0)
            {
                StringOutput = "There is no arguments after the your !math...";
            }
            else
            //There is at least one arguments in the input: 
            {
                StringOutput = evaluteMath(WhatUserAsked);
            }

           return StringOutput;
        }


        protected string evaluteMath(string[] objects)
        {

            string UserSpeaking = Context.NameOfUserSpeaking();

            string formattedouput = "Hi! " + UserSpeaking + " Here is my take on this: \n";

            Interpretor[] replies = new Interpretor[objects.Length];

            for (int i = 0; i < objects.Length; i++)
            {
                replies[i] = new Interpretor(objects[i]);
            }

            foreach (Interpretor itp in replies)
            {
                if (itp.HasErrorOccured())
                {
                    formattedouput
                        +=
                    "```" + itp.inputexpression + "```"
                    +
                        "Is not a valid expression\n"
                    + itp.outputresult + "\n";
                    continue;
                }
                formattedouput +=
                  itp.inputexpression
                  +
                  " = "
                  +
                  itp.outputresult
                  +
                  "\n";
            }

            Utilities.Stuff.ConsoleLog("This is the bot output: ");
            Utilities.Stuff.ConsoleLog(formattedouput);
            return formattedouput;
        }

        /// <summary>
        /// A static method that return a string, which will be used to 
        /// display to the user to show the usage of the command. s
        /// </summary>
        /// <returns></returns>
        public static string getHelpTips(MessageContext mc)
        {

            string formattedreply = "Use: **" + Manager.GetServerCommandPrefix(mc)+"math** to give " +
                "me a math problem, here is an example: \n";

            Random rd = new Random();
            Interpretor temp = new Interpretor(CommandConstant.EXAMPLES[rd.Next(2)]);
            formattedreply += "Your Input: \"" + temp.inputexpression + "\"+\n";
            formattedreply += "The Output will be: " + temp.outputresult;
            return formattedreply;
        }


    }

    #endregion

    #region intro command

    public class TheIntroCommand : ICommandHandles
    {
        protected MessageContext Context;
        protected string NameOfUserSpeakingTo;
        protected string UserAvataID;

        protected string OwnerOfTheServer;
        string ChannelName;
        string ServerName; 
        IEnumerable<GuildEmote> Emotes;
       
        DateTimeOffset ServerTimeStamp; 

        public TheIntroCommand(MessageContext arg)
        {
            
            this.Context = arg;
            NameOfUserSpeakingTo = Context.NameOfUserSpeaking();
            if (!Context.isInServer()) return;

            OwnerOfTheServer = Context.getServerOwner();
            Emotes = Context.getAllEmote();
            IChannel imc =Context.getCurrentChannel();
            ChannelName = imc.Name;
            ServerTimeStamp = Context.getGuildTimeStamp();
            UserAvataID = Context.getUserSpeakingWith().AvatarId;
            ServerName = Context.getServerName();
        }


        /// <summary>
        /// Internal method that return a string full of emote. 
        /// </summary>
        /// <returns></returns>
        protected string getAllEmotesStr()
        {
            string res = ""; 
            IEnumerator<GuildEmote> em = Emotes.GetEnumerator();

            while (em.MoveNext())
            {

                string ge = " <:"+em.Current.Name+":"+em.Current.Id+"> ";
                res += ge;
            }
            return res; 
        }

        /// <summary>
        /// Internal method, for structuring the reply. 
        /// </summary>
        /// <returns></returns>
        protected string getServerLifeTime()
        {
            string res = "";
            DateTime dt = this.ServerTimeStamp.DateTime;
            res += "Date: "+dt.ToString()+" | ";
            res += "Ticks: " + dt.Ticks;
            return res; 
        }

        /// <summary>
        /// Reply for command intro
        /// </summary>
        /// <returns></returns>
        public string getReply()
        {
            //check is it a dm or a guild
            string reply = "Hi,"+this.NameOfUserSpeakingTo +"\n";

            Utilities.Stuff.ConsoleLog(this.Context._subject.Channel.Name);

            if (!this.Context.isInServer())
            {
                reply += "I am Twilight Sparkle, they call me purple smart.";
                return reply;
            };

            string allEmoteStr = this.getAllEmotesStr();

            reply += "I am happy to introduce myself in the " +this.ChannelName+ " Channel.\n";
            reply += "I am purple smart and my real name is Twilight Sparkle.\n";
            reply += "The owner of the server "+this.ServerName+" is: " + this.OwnerOfTheServer +" and I believe " +
                (this.NameOfUserSpeakingTo == this.OwnerOfTheServer?"you":"they") +
                " are very nice\n";
            if (allEmoteStr.Length != 0)
            {
                reply += "Whoa, are emotes in the server!\n";
                reply += allEmoteStr + " \n";
            }

            reply += "There server has TimeStamp: " + this.getServerLifeTime()+"\n";
            reply += "The server has ID: " + this.Context.getServerID()+"\n" ;
            reply += "The command prefix for this server is: \n";
            reply += "```" + Manager.GetServerCommandPrefix(Context) + "```"; 

            Utilities.Stuff.ConsoleLog(reply);

            return reply;
        }

        /// <summary>
        /// A static method that return a string, which will be used to 
        /// display to the user to show the usage of the command. s
        /// </summary>
        /// <returns>
        /// A string for the user, empty string the current command 
        /// context if not server, or the arg is null. 
        /// </returns>
        public static string getHelpTips(MessageContext mcb)
        {

            if (mcb == null ||!mcb.isInServer()) return "" ;

            string result = "Try "+Manager.GetServerCommandPrefix(mcb)+"intro command to see what the bot can say about this server! \n";

            return result; 
        }

    }
    #endregion

    #region help command
    /// <summary>
    /// A class for the execution of TheHelp Command
    /// The class can be instantiated with A contextbridge
    /// to provide context to other command class in the name space. 
    /// </summary>
    public class TheHelpCommand : ICommandHandles
    {

        MessageContext Context; 

        /// <summary>
        /// Contruct the command, please Provide a context 
        /// for the command. 
        /// </summary>
        /// <param name="mcb"></param>
        public TheHelpCommand(MessageContext mcb = null)
        {
            this.Context = mcb; 
        }


        public string getReply()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TheMathCommand.getHelpTips(this.Context));
            sb.Append("\n");
            sb.Append(TheIntroCommand.getHelpTips(this.Context));
            return sb.ToString();
        }


    }
    #endregion

    #region prefix command
    public class PrefixCommand : ICommandHandles
    {
        MessageContext Context;
        /// <summary>
        /// 
        /// A message context with userinput (not RAW MESSAGE PLEASE)
        /// is needed. 
        /// </summary>
        /// <param name="con"></param>
        public PrefixCommand(MessageContext con)
        {
            this.Context = con ?? throw new Exception("You shouldn't do this. "); 

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// A reply for this command. 
        /// </returns>
        public string getReply()
        {
            string[] userinput = Context.getUserInput();
            if (userinput == null)
                Utilities.Stuff.ConsoleLog("NOT GOOD! => Userinput is null if Prefixcommand getreplay method. ");

            // If not in guild, the command shouldn't be envoked, hence guild check avoided. 

            if (userinput.Length == 0||userinput[0]=="")
                return "No parametered for your command, Command is ignored. ";

            string newprefix = userinput[0];
            setPrefix(newprefix);

            string reply = "This is the new prefix: \n";
            reply += "```" + newprefix + "```"; 

            return reply; 
        }

        /// <summary>
        /// Read server for the data base and store the command prefix for the server. 
        /// </summary>
        protected void setPrefix(string newprefix)
        {
            Server sev = new Server(Context._subject.Guild);
            sev.commandprefix = newprefix;
            Manager.AddServer(sev);
        }



        public static string getHelpTips(MessageContext arg)
        {
            if (arg == null) return ""; 
            return ""; 
        }
    }

    #endregion


    /// <summary>
    /// May be an enum class is better? 
    /// </summary>
    public class CommandConstant
    {
        public const string COMPREFIX = ">>>"; 
        public const string HELPSTRING =
            "Use "+COMPREFIX+"math command to solve integer arithmatic.\n"
            +
            "Here is an example: "
            ;

        


        public static readonly string[] EXAMPLES = { "2+9/5-5*(334/45)", "333/452+3/(5*88-6)" };
    }
}
