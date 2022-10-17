using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwsDotnetCdkCloudwatchDashboard
{
    sealed class Program
    {
        public static void Main(string[] args)
        {

            //Update Variables: 
            var volumeId = "vol-xxxxxxxxxxxxxxxxx";
            var awsRegion = "us-east-2";
            var environment = "dev";
            var solutionName = "cloudwatch-monitoring";
            var costcenter = "12_1_12_9_20_8";
            var dashboard = "DemoDashboard";
            

            var app = new App();

            new AwsDotnetCdkCloudwatchDashboardStack(app, "AwsDotnetCdkCloudwatchDashboardStack", new MyStackProps
            {

                Env = new Amazon.CDK.Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = awsRegion 
                },
                StackName = solutionName + "-dashboard-" + environment,
                Description = "Demo Dashboard",
                dashboardName = dashboard,
                solutionName = solutionName, 
                environment = environment, 
                costcenter = costcenter,
                volumeId = volumeId



            }) ;

            Tags.Of(app).Add("costcenter", costcenter); // Add a tag to all constructs in the stack

            app.Synth();
        }
    }
}
