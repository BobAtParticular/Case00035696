using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Sample.Core;

public class Startup
{
    #region ContainerConfiguration

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
	    var container = new WindsorContainer();
	    var registration = Component.For<MyService>()
		    .Instance(new MyService());
	    container.Register(registration);

		NServiceBusConfig.Configure(container);

		return WindsorRegistrationHelper.CreateServiceProvider(container, services);
	}

    #endregion

    public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment environment, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();

        if (environment.IsDevelopment())
        {
            applicationBuilder.UseDeveloperExceptionPage();
        }

        #region RequestHandling

        applicationBuilder.Run(
            handler: context =>
            {
                if (context.Request.Path != "/")
                {
                    // only handle requests at the root
                    return Task.CompletedTask;
                }
                var applicationServices = applicationBuilder.ApplicationServices;
                var endpointInstance = applicationServices.GetService<IEndpointInstance>();
                var myMessage = new MyMessage();

                return Task.WhenAll(
                    endpointInstance.SendLocal(myMessage),
                    context.Response.WriteAsync("Message sent"));
            });

        #endregion
    }
}