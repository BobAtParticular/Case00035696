using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.CustomChecks;

namespace Sample.Core
{
    public class MyCustomCheck : CustomCheck
    {
		public MyService Service { get; set; }
	    public MyCustomCheck() : base("MyCustomCheck-Startup", "MyCustomCheck-Category")
	    {
	    }

		public override Task<CheckResult> PerformCheck()
		{
			Service.WriteCheck();
			return Task.FromResult(CheckResult.Pass);
		}
	}
}
