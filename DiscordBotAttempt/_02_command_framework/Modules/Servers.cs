using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotAttempt._02_command_framework.Modules
{
    class Servers
    {

        protected ISet<Servers> RemeberedServer;

        // These are min of data that can identify a server. 
        protected ulong _ownerID;
        protected string ServerName; 
    }
}
