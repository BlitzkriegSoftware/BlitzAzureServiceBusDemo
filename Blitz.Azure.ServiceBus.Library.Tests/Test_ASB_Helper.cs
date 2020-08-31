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

        #region "Handlers"

        private DummyModel newMessageHandler(DummyModel model, out bool isOk)
        {
            isOk = true;
            logger.LogInformation($"Happy: {model}");
            testContext.WriteLine($"Happy: {model}");
            IsRunning = false;
            return model;
        }

        private DummyModel unhappyMessageHandler(DummyModel model, out bool isOk)
        {
            isOk = false;
            logger.LogInformation($"Unhappy: {model}");
            testContext.WriteLine($"Unhappy: {model}");
            IsRunning = false;
            return model;
        }

        private string badMessageHandler(string message)
        {
            logger.LogInformation($"Error: {message}");
            testContext.WriteLine($"Error: {message}");
            IsRunning = false;
            return message;
        }

        #endregion

        [TestMethod]
        public void T_Client_1()
        {
            IsRunning = true;
            var client = new AzureServiceBusHelper<Models.DummyModel>(asbConnectionString, logger);

            var model = new Models.DummyModel();

            client.Enqueue(model, QueueName, model.Id);
            client.Dequeue(QueueName, newMessageHandler, badMessageHandler);

            while (IsRunning) { }
        }

        [TestMethod]
        public void T_Client_2()
        {
            IsRunning = true;
            var client = new AzureServiceBusHelper<Models.DummyModel>(asbConnectionString, logger);

            var model = new Models.DummyModel();

            client.Enqueue(model, QueueName, model.Id);
            client.Dequeue(QueueName, unhappyMessageHandler, badMessageHandler);

            while (IsRunning) { }
        }

    }
}
