using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus.CustomChecks;

public class ExampleCheck : CustomCheck
{
	public ILogger<ExampleCheck> Logger { get; set; }

	public ExampleCheck() : base("ExampleCheck-Startup", "ExampleCheck-Category")
	{
	}

	public override Task<CheckResult> PerformCheck()
	{
		Logger.LogInformation("Message from the ILogger for ExampleCheck");
		return Task.FromResult(CheckResult.Pass);
	}
}