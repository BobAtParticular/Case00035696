using System;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Logging;

class Program
{
	static async Task Main()
	{
		Console.Title = "Case00035696";

		//Initialize the ServiceCollection
		var serviceCollection = new ServiceCollection();

		//Add logging to the ServiceCollection
		var loggerFactory = new LoggerFactory();
		serviceCollection.AddSingleton(loggerFactory.AddConsole());
		serviceCollection.AddLogging();

		//Initialize you Windsor container
		var container = new WindsorContainer();

		//Configure Windsor and the ServiceCollection, now the ServiceCollection registrations are added to the Windsor container
		WindsorRegistrationHelper.CreateServiceProvider(container, serviceCollection);

		//Minimal NSB config
		var configuration = new EndpointConfiguration("Case00035696");
		configuration.UsePersistence<LearningPersistence>();
		configuration.UseTransport<LearningTransport>();

		//Configure NSB to use MS logging
		LogManager.Use<MicrosoftLogFactory>().UseMsFactory(loggerFactory);

		//Configure NSB to use Windsor
		configuration.UseContainer<WindsorBuilder>(c => c.ExistingContainer(container));

		//Configure NSB to enable CustomChecks
		configuration.ReportCustomChecksTo("ServiceControl");

		//At this point the logger factory and everything your NSB checks, handlers, etc need should be registered in the container
		//used by NSB in order ensure they are all initialized correctly and avoid initialization race conditions.

		//Start the endpoint
		var endpointInstance = await Endpoint.Start(configuration);

		try
		{
			Console.WriteLine("Press any key to stop the reproduction");
			Console.ReadKey();
		}
		finally
		{
			await endpointInstance.Stop()
				.ConfigureAwait(false);
		}
	}
}