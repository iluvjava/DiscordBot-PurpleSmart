using NUnit.Framework;
using System.Collections.Generic;
using Services.DataStoreage;
using System;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }




        /// <summary>
        /// Test reading of bot Token 
        /// </summary>
        [Test]
        public void test1()
        {
           string s =  Utilities.Stuff.GetBotToken();
            Utilities.Stuff.ConsoleLog(s);
        }


        /// <summary>
        /// Testing the objeccache class in utilities. 
        /// </summary>
        [Test]
        public void testObjectReadWrite()
        {
            List<string> alist = new List<string>();
            alist.Add("First element. ");
            alist.Add("Second element. ");
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            dir = "";
            ObjectCache<List<string>> storelist =new ObjectCache<List<string>>(alist,dir,"alist.txt");

            Utilities.Stuff.ConsoleLog("Storing list, bool: "+storelist.serialize());
            Utilities.Stuff.ConsoleLog("Reading status: "+storelist.deserialize());

            alist = storelist.ObjectToStore;
            foreach (string str in alist)
            {
                Utilities.Stuff.ConsoleLog(str);
            }

        }
    }
}