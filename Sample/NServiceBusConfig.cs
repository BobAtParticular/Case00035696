using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NServiceBus;

namespace Sample.Core
{
    public class NServiceBusConfig
    {
	    public static void Configure(IWindsorContainer container)
	    {
		    var endpointConfiguration = new EndpointConfiguration("Case00035696");
		    endpointConfiguration.UseTransport<LearningTransport>();
		    endpointConfiguration.UseContainer<WindsorBuilder>(
			    customizations: customizations =>
			    {
				    customizations.ExistingContainer(container);
			    });

		    endpointConfiguration.ReportCustomChecksTo("Particular.ServiceControl");

			var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

		    container.Register(Component.For<IEndpointInstance>().Instance(endpoint));
		}
    }
}
