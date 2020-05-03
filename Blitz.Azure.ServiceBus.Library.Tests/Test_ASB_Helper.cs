using System.Diagnostics.CodeAnalysis;
using Blitz.Azure.ServiceBus.Library.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Blitz.Azure.ServiceBus.Library.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Test_Helper
    {
        #region "Boilerplate"

        private const string QueueName = "Test";

        private static TestContext testContext;
        private static string asbConnectionString;
        private static ILogger logger;
        private static bool IsRunning = false;

        [ClassInitialize]
        public static void InitClass(TestContext testContext)
        {
            Test_Helper.testContext = testContext;

            var json = System.IO.File.ReadAllText(@"testconfig.json");
            var config = JsonConvert.DeserializeObject<Models.AsbConfig>(json);
            asbConnectionString = config.ConnectionString;

            var host = new HostBuilder()
            .ConfigureServices((hostingContext, services) =>
             {
                 services.AddLogging(logger => {
                     logger.SetMinimumLevel(LogLevel.Trace);
                     logger.AddConsole();
                 });

             }).Build();

            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            logger = loggerFactory.CreateLogger<Test_Helper>();
        }

        #endregion

        [TestMethod]
        public void T_Client_1()
        {
            IsRunning = true;
            var client = new AzureServiceBusHelper<Models.DummyModel>(asbConnectionString, logger);

            var model = new Models.DummyModel();

            client.Enqueue(model, QueueName, model.Id);
            client.Dequeue(QueueName, newMessageDelegate);

            while (IsRunning) { }
        }

        private DummyModel newMessageDelegate(DummyModel model)
        {
            logger.LogInformation($"{model}");
            testContext.WriteLine($"{model}");
            IsRunning = false;
            return model;
        }
    }
}
