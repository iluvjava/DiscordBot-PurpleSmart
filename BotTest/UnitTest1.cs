using NUnit.Framework;


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
           string s =  Utilities.Utilities.GetBotToken();
            Utilities.Utilities.ConsoleLog(s);
        }
    }
}