using Discord.Commands;
using DiscordBotAttempt._02_command_framework.Modules;
using MathyStuff;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

    #region Command math
    /// <summary>
    /// This is the math command we want to focus on. 
    /// </summary>
    /// 
    public class TheMathCommand : ICommandHandles
    {
        protected MessageContextBridge Context;

        public TheMathCommand(MessageContextBridge arg)
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
                    "\"" + itp.inputexpression + "\""
                    +
                        "Is not a valid expression\n"
                    +itp.outputresult + "\n";
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

            Console.WriteLine("This is the bot output: ");
            Console.WriteLine(formattedouput);

            return formattedouput;
        }
    }

    #endregion


}
