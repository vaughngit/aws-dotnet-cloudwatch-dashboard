using Amazon.CDK;
using Amazon.CDK.AWS.CloudWatch;
using Constructs;
using System.Collections.Generic;

namespace AwsDotnetCdkCloudwatchDashboard
{
    public class MyStackProps : StackProps
    {
        public string costcenter {get; set; }
        public string dashboardName { get; set; }
        public string solutionName { get; set; }
        public string environment { get; set; }
        public string volumeId { get; set; }
    }
    public class AwsDotnetCdkCloudwatchDashboardStack : Stack
    {
        internal AwsDotnetCdkCloudwatchDashboardStack(Construct scope, string id, MyStackProps props = null) : base(scope, id, props)
        {
            //Volume Reads Metric :
            Metric volumeReadOpsMetric = new Metric(new MetricProps
            {
                MetricName = "VolumeReadOps",
                Namespace = "AWS/EBS",
                Label = "DemoVolumeReadOperations",
                DimensionsMap = new Dictionary<string, string> {
                    { "VolumeId", props.volumeId }
                },
                Statistic = "Sum",
                Period = Duration.Minutes(1)

            });

            //Volume Reads Metric :
            Metric volumeWriteOpsMetric = new Metric(new MetricProps
            {
                MetricName = "VolumeWriteOps",
                Namespace = "AWS/EBS",
                Label = "DemoVolumeWriteOperations",
                DimensionsMap = new Dictionary<string, string> {
                    { "VolumeId", props.volumeId }
                },
                Statistic = "Sum",
                Period = Duration.Minutes(1)

            });

            //Calculate IOPS: 
            MathExpression volumeIopsCalc = new MathExpression(new MathExpressionProps
            {
                Expression = "(readOps+writeOps)/60",
                Label = "Volume IOPS",
                UsingMetrics = new Dictionary<string, IMetric> {
                    { "readOps", volumeReadOpsMetric },
                    { "writeOps", volumeWriteOpsMetric }
                },
                Period = Duration.Minutes(1)
            }) ;

            ////////////////////////////////////////////////////////////////////////////
            //Measure Throughput 

            //Volume Reads Metric :
            Metric volumeReadThroughputMetric = new Metric(new MetricProps
            {
                MetricName = "VolumeReadBytes",
                Namespace = "AWS/EBS",
                Label = "DemoVolumeReadThroughput",
                DimensionsMap = new Dictionary<string, string> {
                    { "VolumeId", props.volumeId }
                },
                Statistic = "Sum",
                Period = Duration.Minutes(1)

            });

            //Volume Reads Metric :
            Metric volumeWriteThroughputMetric = new Metric(new MetricProps
            {
                MetricName = "VolumeWriteBytes",
                Namespace = "AWS/EBS",
                Label = "DemoVolumeWriteThroughput",
                DimensionsMap = new Dictionary<string, string> {
                    { "VolumeId", props.volumeId }
                },
                Statistic = "Sum",
                Period = Duration.Minutes(1)

            });

            //Calculate Throughput  
            //Throughput in Mbs = '(Total Read Throughput bytes + Total Read Throughput bytes)/60/1024/1024' 
            MathExpression volumeThroughputCalc = new MathExpression(new MathExpressionProps
            {
                Expression = "(read+write)/60/1024/1024",
                Label = "Volume Throughput in MBs/Sec",
                UsingMetrics = new Dictionary<string, IMetric> {
                    { "read", volumeReadThroughputMetric },
                    { "write", volumeWriteThroughputMetric }
                },
                Period = Duration.Minutes(1)
            });



            //The CloudWatch Dashboard 
            ///////////////////////////////////////////////////////////////////////////////
            // Create CloudWatch Dashboard

            Dashboard dashboard = new Dashboard(this, "cwdashboard", new DashboardProps
            {
                DashboardName = props.dashboardName,
               
            });

            dashboard.AddWidgets(new TextWidget(new TextWidgetProps
            {
                Markdown = "# Dashboard: Instance Volume Performance",
                Height= 1,
                Width = 24
            }));

            //IOPS Widget
            dashboard.AddWidgets(new GraphWidget(new GraphWidgetProps {
                Title = "Volume IOPS Per Minute",
                Width = 24, 
                Left = new[] { volumeIopsCalc },
                LiveData = true, 
                Statistic = "Average",
                View = GraphWidgetView.TIME_SERIES,
                Period = Duration.Minutes(1)
            }));

            //Throughput Widget
            dashboard.AddWidgets(new GraphWidget(new GraphWidgetProps
            {
                Title = "Volume Throughput Per Minute",
                Width = 24,
                Left = new[] { volumeThroughputCalc.With(new MathExpressionOptions {
                    Color = Color.GREEN,
                }) },
                LiveData = true,
                Statistic = "Average",
                View = GraphWidgetView.TIME_SERIES,
                Period = Duration.Minutes(1)
            }));


            //Dashboard URL: 
            ////////////////////////////////////////////////////////////////

            new CfnOutput(this, "DashboardURL", new CfnOutputProps {
                Value = $"https://{this.Region}.console.aws.amazon.com/cloudwatch/home?region={this.Region}#dashboards:name={props.dashboardName}"
            });  


        }

    }
}
