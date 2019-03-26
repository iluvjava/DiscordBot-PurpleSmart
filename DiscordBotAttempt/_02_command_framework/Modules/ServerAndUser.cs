using Discord;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Modules
{

    /// <summary>
    /// A Server class is a class that we can use 
    /// to store info about the server, it's properties
    /// its members, and should be able to identify all of them.
    /// <para>
    /// Command prefix
    /// </para>
    /// </summary>
    public class Server
    {
        MyEntity IDcard;

        // The command prefix set in this server. 
        string commandprefix; 

        protected ISet<Server> RemeberedServer;
        // These are min of data that can identify a server. 
        public Server(IGuild server)
        {
            IEntity<ulong> temp = server as IEntity<ulong>;
            this.IDcard = new MyEntity(server.Name,temp.Id);
        }



      

    }

    /// <summary>
    /// A struct that represents something that is, private for internal use. 
    /// count as identity on the internet. 
    /// </summary>
    [Serializable]
    struct MyEntity:ISerializable
    {
        public ulong ID;
        public string Name;

        public MyEntity(string Name, ulong id)
        {
            ID = id;
            this.Name = Name;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }

    

}
