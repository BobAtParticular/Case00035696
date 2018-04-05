using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus.CustomChecks;

namespace Sample.Core
{
    public class MyCustomCheck : CustomCheck
    {
	    public ILogger<MyCustomCheck> Logger { get; set; }
		public MyService Service { get; set; }

	    public MyCustomCheck() : base("MyCustomCheck-Startup", "MyCustomCheck-Category")
	    {
	    }

		public override Task<CheckResult> PerformCheck()
		{
			Logger.LogInformation("Logger Message",1);
			Service.WriteCheck();
			return Task.FromResult(CheckResult.Pass);
		}
	}
}
