using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Windsor;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Core;

static class Program
{
    static async Task Main()
    {
        var webHostBuilder = new WebHostBuilder();
        webHostBuilder.UseKestrel();
        webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
        webHostBuilder.UseIISIntegration();
	    webHostBuilder.ConfigureServices(serviceCollection => serviceCollection.AddSingleton(new WindsorContainer()));
	    webHostBuilder.ConfigureServices(serviceCollection =>
		    serviceCollection.AddSingleton<IHostedService, NServiceBusBackgroundService>());
        webHostBuilder.UseStartup<Startup>();
        var host = webHostBuilder.Build();
        await host.StartAsync()
            .ConfigureAwait(false);
        var serverAddresses = host.ServerFeatures.Get<IServerAddressesFeature>();
        var address = serverAddresses.Addresses.First();
        Console.WriteLine($"Now listening on: {address}");
        Console.WriteLine("Press any key to shutdown");

        AttemptToLaunchBrowser(address);

        Console.ReadKey();
        await host.StopAsync()
            .ConfigureAwait(false);
    }

    static void AttemptToLaunchBrowser(string address)
    {
        Console.WriteLine($"Attempting to open browser to: {address}");
        try
        {
            using (Process.Start("explorer.exe", address))
            {
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Failed to launch browser. Open manually: {address}");
        }
    }
}