using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocGoogleAnalytics
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GoogleAnatyticsTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ex: " + ex);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void GoogleAnatyticsTest()
        {
            var filepath = @"C:\Users\tkrchnak\source\repos\PocGoogleAnalytics\PocGoogleAnalytics\GA-test-da6d26d5f8e0.json";  // path to the json file for the Service account
            var viewid = "180768931";    // id of the view you want to read from
            GoogleCredential credentials;
            using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly };
                var googleCredential = GoogleCredential.FromStream(stream);
                credentials = googleCredential.CreateScoped(scopes);
            }

            var reportingService = new AnalyticsReportingService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credentials
                });
            var dateRange = new DateRange
            {
                StartDate = "2016-07-01",
                EndDate = "2019-01-15"
            };
            var sessions = new Metric
            {
                Expression = "ga:pageviews",
                Alias = "Sessions"
            };
            var pagePath = new Dimension { Name = "ga:pagePath" };

            var reportRequest = new ReportRequest
            {
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { pagePath },
                Metrics = new List<Metric> { sessions },
                ViewId = viewid
            };
            var getReportsRequest = new GetReportsRequest
            {
                ReportRequests = new List<ReportRequest> { reportRequest }
            };
            var batchRequest = reportingService.Reports.BatchGet(getReportsRequest);
            var response = batchRequest.Execute();
            foreach (var x in response.Reports.First().Data.Rows)
            {
                Console.WriteLine(string.Join(", ", x.Dimensions) + "   " + string.Join(", ", x.Metrics.First().Values));
            }
        }
    }
}
