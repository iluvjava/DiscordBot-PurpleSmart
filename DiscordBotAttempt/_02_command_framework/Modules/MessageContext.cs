using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modules
{

    /// <summary>
    /// This is a class that will possibly bridge some of things 
    /// in the command context. 
    /// 
    /// </summary>
   public class MessageContext
   {
        public ICommandContext _subject { get; set; }

        protected ulong _botID;

        protected string[] userInputs;

        /// <summary>
        /// In order to execute a command perfectly, 
        /// we need the context of the command and what does the 
        /// user said specifically. 
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="Userinputs"></param>
        public MessageContext(ICommandContext commandcontext, string[] Userinputs = null)
        {
            this.userInputs= Userinputs;
            _subject = commandcontext ?? throw new Exception("Null Command Context."); 
        }


        /// <summary>
        /// In order to execute a command perfectly, 
        /// we need the context of the command and what does the 
        /// user said specifically, this case, it's a single inputs
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="Userinputs"></param>
        public MessageContext(ICommandContext arg, string userinput) :this(arg)
        {
            string[] temp = new string[1];
            temp[1] = userinput;
            this.userInputs = temp;
        }


        /// <summary>
        /// This method return the string input by the user after command. 
        /// So if this return null, then it means 
        /// you don't need specific arguments for the command. 
        /// </summary>
        /// <returns>
        /// String[] that it's input by the user,
        /// null if the command doesn't need any arguments or the user 
        /// has the incorrect input. 
        /// </returns>
        public string[] getUserInput()
        {
            return this.userInputs;
        }


        /// <summary>
        /// The fault channel id points the the first viewable text channel in the 
        /// server. 
        /// </summary>
        /// <returns>
        /// Null if the current context is not in a server. 
        /// </returns>
        public ulong GetServerdefaultChannelID()
        {
            if (_subject == null) return 0;
            ulong result = _subject.Guild.DefaultChannelId;
            return result;
        }

        /// <summary>
        /// This method return all the emote in ther server
        /// </summary>
        /// <returns>
        /// Null if he current context is not a server or something wrong 
        /// </returns>
        public IEnumerable<GuildEmote> getAllEmote()
        {
            return _subject.Guild.Emotes;
        }

        /// <summary>
        /// Return the name of the server owner 
        /// </summary>
        /// <returns>
        /// Null if there is something wrong. 
        /// </returns>
        public string getServerOwner()
        {
            if (_subject.Guild == null) return null; 
            ulong id =_subject.Guild.OwnerId;
            Task<IGuildUser> temp = _subject.Guild.GetUserAsync(id);
            temp.Wait();

            IGuildUser user = temp.Result;
            return user.Username;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// The name of the user in the context of the 
        /// conversation. 
        /// </returns>
        public string NameOfUserSpeaking()
        {
            return _subject.User.Username;
        }
        /// <summary>
        /// Get Iuser of the user speaking to, could be from dm
        /// could be from server. 
        /// </summary>
        /// <returns></returns>
        public IUser getUserSpeakingWith()
        {
            return _subject.User;
        }

        /// <summary>
        /// Get IUserMessage! 
        /// </summary>
        /// <returns></returns>
        public IUserMessage GetUserMessage()
        {
            return _subject.Message;
        }


        /// <summary>
        /// REturn the id of the bot itself. 
        /// </summary>
        /// <returns></returns>
        public ulong botID()
        {
            if (this._botID != 0) return _botID;

            //save it in the field for this context. 
            ulong id = this._subject.Client.CurrentUser.Id;
            this._botID = id; 
            return id; 
        }



        /// <summary>
        /// a bool to see if the bot is mentioned in the context of the command 
        /// or conversation. 
        /// </summary>
        /// <returns>
        /// true if the bot is mentioned. 
        /// </returns>
        public bool BotIsMentioned()
        {
            IEnumerable<ulong> mentionedID = this._subject.Message.MentionedUserIds;
            IEnumerator<ulong> e = mentionedID.GetEnumerator();
            while (e.MoveNext())
            {
                ulong item = e.Current;
                if (item == this.botID())
                {
                    return true; 
                }
            }
            return false; 
        }


        /// <summary>
        /// Return an IMessage Channel extracted from the chat context. 
        /// </summary>
        /// <returns></returns>
        public IMessageChannel getCurrentChannel()
        {
            IMessageChannel imc =this._subject.Channel;
            return imc; 
        }


        /// <summary>
        /// Return A SoketTextChannel Object if the current 
        /// channel the context is on is an instance of SocketTextChannel. 
        /// </summary>
        /// <returns>
        /// A Socket TextChanell object in instance, null if the 
        /// current IMessage Channel cannot be cast to 
        /// SocketTextChannel. 
        /// </returns>
        public SocketTextChannel getSocketTextChannel()
        {
            if (this.getCurrentChannel() is SocketTextChannel)
            {
                return this.getCurrentChannel() as SocketTextChannel;
            }
            return null;
        }


        /// <summary>
        /// Return a DateTimOffset object, which is 
        /// a unique identifier for the server. 
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset getGuildTimeStamp()
        {
           ISnowflakeEntity sfid =  this._subject.Guild;
            DateTimeOffset timestamp = sfid.CreatedAt;
            return timestamp;
        }

        /// <summary>
        /// True if the command is executed in a server, false if it's not. 
        /// </summary>
        /// <returns></returns>
        public bool isInServer()
        {
            return _subject.Guild != null;
        }

        /// <summary>
        /// Return the name of the server
        /// </summary>
        /// <returns>
        /// null if the current command is not identified in a server 
        /// context.
        /// </returns>
        public string getServerName()
        {
            if (isInServer()) return this._subject.Guild.Name;
            return null;
        }


        /// <summary>
        /// This class is more comprehensive version of the 
        /// class above, it requires more context, and 
        /// it should be used when we have both the raw message 
        /// and has constructed our command context. 
        /// </summary>
        public class RawMessageContextBridge :MessageContext
        {
            SocketMessage _rawmessage { get; set; }

            public RawMessageContextBridge(SocketMessage rawmessage, ICommandContext commandmessage) : base(commandmessage)
            {


            }



        }


    }
}
