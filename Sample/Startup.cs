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

public class Startup
{
	public WindsorContainer Container { get; }

	public Startup(WindsorContainer container)
	{
		Container = container;
	}


    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
	    var registration = Component.For<MyService>()
		    .Instance(new MyService());
	    Container.Register(registration);

		return WindsorRegistrationHelper.CreateServiceProvider(Container, services);
	}

	public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment environment, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();

	    Container.Register(Component.For(typeof(ILogger<>))
		    .UsingFactoryMethod((k, c, t) => loggerFactory.CreateLogger(t.RequestedType.GenericTypeArguments[0])));

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
                var messageSession = applicationServices.GetService<IMessageSession>();
                var myMessage = new MyMessage();

                return Task.WhenAll(
	                messageSession.SendLocal(myMessage),
                    context.Response.WriteAsync("Message sent"));
            });

        #endregion
    }
}