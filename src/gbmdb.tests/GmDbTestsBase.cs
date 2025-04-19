using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{

    [TestClass]
    public class GmDbTestsBase
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Helper methods

        protected void Log(string strFormat, params object[] aobjArgs)
        {
            Console.Write("{0}: ", DateTime.Now);
            Console.WriteLine(strFormat, aobjArgs);
        }

        #endregion

        protected DateTime dtStart = default(DateTime);
        protected DateTime dtStop = default(DateTime);

        protected static string GmPath { get; set; }
        protected static string GmUserData { get; set; }

        public GmDbTestsBase()
        {
            GmPath = @"D:\GM";
            GmUserData = "USERDATA";
        }
    }
}
