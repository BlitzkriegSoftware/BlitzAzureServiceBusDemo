using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Blitz.Azure.ServiceBus.Library.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Test_Helper
    {
        #region "Boilerplate"

        private static TestContext Context;
        private static string ASB_ConnectionString;

        [ClassInitialize]
        public static void InitClass(TestContext testContext)
        {
            Context = testContext;

            var json = System.IO.File.ReadAllText(@"testconfig.json");
            dynamic config = JArray.Parse(json);
            ASB_ConnectionString = config.connectionstring;
        }

        #endregion

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
