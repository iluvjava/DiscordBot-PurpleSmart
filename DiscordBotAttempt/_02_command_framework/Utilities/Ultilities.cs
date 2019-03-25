﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{

    /// <summary>
    /// This contains console loging. 
    /// </summary>
    public class Utilities
    {
        public static void ConsoleLog(Object o)
        {
            Console.WriteLine(o.ToString());
        }


        /// <summary>
        /// It gets the bot token from my personal PC. 
        /// </summary>
        /// <returns>
        /// Null is there is something wrong. 
        /// </returns>
        public static string GetBotToken()
        {
            string token = null; 
            try
            {
               string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                token = System.IO.File.ReadAllText(path+@"\discordbottoken.txt");
            }
            catch (Exception e)
            {
                ConsoleLog(e.Message);
            }
            return token;
        }

    }
}
