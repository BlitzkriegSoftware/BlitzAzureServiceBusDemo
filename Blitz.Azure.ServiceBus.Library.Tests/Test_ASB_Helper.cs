using System;
using System.Collections.Generic;
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
        private static Random dice = new Random();
        private static List<Guid> Messages;

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

            Messages = new List<Guid>();
        }

        #endregion

        #region "Handlers"

        private DummyModel newMessageHandler(DummyModel model, out bool isOk)
        {
            isOk = true;
            logger.LogInformation($"Happy: {model}");
            testContext.WriteLine($".Happy: {model}");
            IsRunning = false;
            return model;
        }

        private DummyModel unhappyMessageHandler(DummyModel model, out bool isOk)
        {
            isOk = false;
            logger.LogInformation($"Unhappy: {model}");
            testContext.WriteLine($".Unhappy: {model}");
            IsRunning = false;
            return model;
        }

        private string badMessageHandler(string message)
        {
            logger.LogInformation($"Error: {message}");
            testContext.WriteLine($".Error: {message}");
            IsRunning = false;
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isOk"></param>
        /// <returns></returns>
        private DummyModel newManyMessageHandler(DummyModel model, out bool isOk)
        {

            if (dice.Next(0,100) <= 80)
            {
                logger.LogInformation($"Happy: {model}");
                testContext.WriteLine($".Happy: {model}"); 
                if(Messages.Contains(model.Id))
                {
                    Messages.Remove(model.Id);
                }
                isOk = true;
            }
            else
            {
                logger.LogInformation($"Not Ok: {model}");
                testContext.WriteLine($".Not Ok: {model}"); 
                isOk = false;
            }
            return model;
        }

        private string badManyMessageHandler(string message)
        {
            logger.LogInformation($"Error: {message}");
            testContext.WriteLine($".Error: {message}");
            return message;
        }

        #endregion

        [TestMethod]
        [TestCategory("Integration")]
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
        [TestCategory("Integration")]
        public void T_Client_2()
        {
            IsRunning = true;
            var client = new AzureServiceBusHelper<Models.DummyModel>(asbConnectionString, logger);

            var model = new Models.DummyModel();

            client.Enqueue(model, QueueName, model.Id);
            client.Dequeue(QueueName, unhappyMessageHandler, badMessageHandler);

            while (IsRunning) { }
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void ManyMessages1()
        {
            var ct = 6;
            Messages.Clear();

            var client = new AzureServiceBusHelper<Models.DummyModel>(asbConnectionString, logger);
            client.Dequeue(QueueName, newManyMessageHandler, badManyMessageHandler);

            for (int i = 0; i < ct; i++)
            {
                var model = new Models.DummyModel();
                Messages.Add(model.Id);
                client.Enqueue(model, QueueName, model.Id);
            }

            while (Messages.Count  > 0) { }
        }


    }
}
