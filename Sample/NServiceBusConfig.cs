using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Sample.Core
{
    public class NServiceBusBackgroundService : IHostedService
    {
	    public IWindsorContainer Container { get; }
	    private IEndpointInstance endpoint;

	    public NServiceBusBackgroundService(IWindsorContainer container)
	    {
		    Container = container;
	    }

	    public async Task StartAsync(CancellationToken cancellationToken)
	    {
		    var endpointConfiguration = new EndpointConfiguration("Case00035696");
		    endpointConfiguration.UseTransport<LearningTransport>();
		    endpointConfiguration.UseContainer<WindsorBuilder>(
			    customizations: customizations =>
			    {
				    customizations.ExistingContainer(Container);
			    });

		    endpointConfiguration.ReportCustomChecksTo("Particular.ServiceControl");

		    endpoint = await Endpoint.Start(endpointConfiguration);

		    Container.Register(Component.For<IMessageSession>().Instance(endpoint));
		}

	    public async Task StopAsync(CancellationToken cancellationToken)
	    {
		    if (endpoint != null)
		    {
			    await endpoint.Stop();
		    }
	    }
    }
}
