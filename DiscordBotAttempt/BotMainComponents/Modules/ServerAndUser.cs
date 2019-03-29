using Discord;
using Services.DataStoreage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Modules
{

    /// <summary>
    /// THis is a class that provides helpful methods 
    /// and fields that magage the server and 
    /// user class. 
    /// <para>
    /// This class is going to be all static! 
    /// </para>
    /// </summary>
    public class Manager
    {

        /// <summary>
        /// This method load data and create exist event. 
        /// </summary>
        public static void PrepareEverthing()
        {
            Utilities.Stuff.ConsoleLog("Preparing DataBase...");

            Manager.ReadServers();
            //Add An exist event. 
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

        }


        
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Manager.SaveServers();
            Console.WriteLine("Existing and saving... ");
            Thread.Sleep(1000);
        }


        //A set of servers that the bot has memorize. 
        public static HashSet<Server> ServerSet
        {
            get;
            protected set;
        } 

        /// <summary>
        /// A static methods that read a list of 
        /// servers that has been stored in the hardisk. 
        /// </summary>
        /// <returns></returns>
        public static void ReadServers()
        {
            ObjectCache<HashSet<Server>>
                stuff
            =
            new ObjectCache<HashSet<Server>>(new HashSet<Server>(), "", "ServerSet.txt");
            bool status = stuff.deserialize();
            Manager.ServerSet = stuff.ObjectToStore;
        }


        /// <summary>
        /// Transfer all the servers from memory to the hardisk. 
        /// </summary>
        /// <returns>
        /// true or false to indicate how the operation goes. 
        /// </returns>
        public static Task<bool> SaveServers()
        {
            Task<bool> t = Task<bool>.Factory.StartNew
           (
                () =>
                {
                    ObjectCache<HashSet<Server>> stufftosave
                    =
                    new ObjectCache<HashSet<Server>>(Manager.ServerSet, "", "ServerSet.txt");
                    return stufftosave.serialize();
                }
            );
            return t;
        }

        /// <summary>
        /// Add a new server to the ISet, the set of all servers. 
        /// </summary>
        /// <param name="sev"></param>
        public static void AddServer(IGuild sev)
        {
            Manager.AddServer(new Server(sev));
        }


        /// <summary>
        /// Add a new server to the ISet, the set of all servers. 
        /// </summary>
        /// <param name="sev"></param>
        public static void AddServer(Server sev)
        {
            Utilities.Stuff.ConsoleLog("Manager Adding Server: "+ sev);
            //remove and add again if it's already there. 
            if (!Manager.ServerSet.Add(sev))
            {
                Manager.ServerSet.Remove(sev);
                Utilities.Stuff.ConsoleLog("Execution Status: "+Manager.ServerSet.Add(sev));
            };
        }

        /// <summary>
        /// Get a server object by giving a Guild server. 
        /// </summary>
        /// <param name="sev"></param>
        /// <return>
        /// Null if the server is not in the set. 
        /// </return>
        public static Server GetServer(IGuild sev)
        {
            Server res =null;
            ServerSet.TryGetValue(new Server(sev),out res);
            return res; 
        }

        /// <summary>
        /// Given a IGuid object, it searches for the command from the database, 
        /// if it's a DM message, then just give it a null and it will handle it. 
        /// </summary>
        /// <param name="sev"></param>
        /// <returns></returns>
        public static string GetServerCommandPrefix(IGuild sev)
        {

            //if sev is null, then it's a DM
            if (sev == null) return ">>>";

            Server res = null;
            //Res should be overwritten if it's registered in the set. 
            res = ServerSet.TryGetValue(new Server(sev), out res)?res:new Server(sev);

            return res.commandprefix;

        }



        public static string GetServerCommandPrefix(MessageContext context)
        {
            Server res = null;
            //if It's a DM context
            if (context._subject.Guild == null) return ">>>"; 

            IGuild sev = context._subject.Guild;
            //Res should be overwritten if it's registered in the set. 
            res = ServerSet.TryGetValue(new Server(sev), out res) ? res : new Server(sev);
            return res.commandprefix;

        }




    }

    /// <summary>
    /// A Server class is a class that we can use 
    /// to store info about the server, the Ulong ID
    /// is the only unique identifier for 
    /// a certain sever. 
    /// <para>
    /// Command prefix
    /// </para>
    /// </summary>
    [Serializable]
    public class Server
    {
        //A unique idenfication for the server. 
        public MyEntity IDcard { get; set; }


        // The command prefix set in this server, it's not a unique identifier
        public string commandprefix
        {
            get; set;
        } = ">>>";


        protected ISet<Server> RemeberedServer;
        // These are min of data that can identify a server. 
        public Server(IGuild server)
        {
            IEntity<ulong> temp = server as IEntity<ulong>;
            this.IDcard = new MyEntity(server.Name, temp.Id);
        }

        /// <summary>
        /// A constructor for dummy server. Meant for unit Testing. (and XML serializing)
        /// </summary>
        /// <param name="id"></param>
        protected Server()
        {
            
        }


        /// <summary>
        /// This method is only meant for unit Testing. 
        /// </summary>
        /// <returns></returns>
        public static Server CreateDummyServerForTesting()
        {
            Server dummy= new Server();
            dummy.IDcard = new MyEntity("Dummy", 1234567890);
            return dummy;
        }

        /// <summary>
        /// Compare to server information. 
        /// </summary>
        /// <returns></returns>
        override
        public bool Equals(object obj)
        {
            if (!(obj is Server)) return false;
            Server s = obj as Server;
            {
                if (!s.IDcard.Equals(this.IDcard)) return false;
            }
            return true; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// The ulong ID of the server cast to 
        /// int. 
        /// </returns>
        override
        public int GetHashCode()
        {
            return (int)this.IDcard.ID; 
        }

        override
        public string ToString()
        {
            string s = this.IDcard + "\n";
            s += "Type: Server\n";
            s += "Server name: " + this.IDcard.Name+"\n";
            s += "Server Command Prefix = " + this.commandprefix+"\n";
            return s; 
        }
    }

    /// <summary>
    /// A struct that represents something that is, private for internal use. 
    /// count as identity on the internet. 
    /// </summary>
    [Serializable]
    public struct MyEntity
    {
        //ID is the unique identifier for entities on 
        //discord server. 
        public ulong ID { get; set; }
        public string Name { get; set; }

        public MyEntity(string Name, ulong id)
        {
            ID = id;
            this.Name = Name;
        }

        override
        public bool Equals(object obj)
        {
            if (!(obj is MyEntity)) return false;
            MyEntity AnotherEntity = (MyEntity)obj;

            if (AnotherEntity.ID != this.ID) return false;
            return true; 
        }

        override
        public int GetHashCode()
        {
            return (int)this.ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override
        public string ToString()
        {
            string s = "Discord ulong ID: " + this.ID;
            return s; 
        }
    }


    class MyUser
    {


    }
    

}
