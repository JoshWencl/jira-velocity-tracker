using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadData.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReadData.Utilities
{
    public static class JiraHttpRequest
    {
        static string auth = "";
        static string token = "";
        static string url = "";
        static string workitemPrefix = "";

        public static List<Request> getWorkItemObjects(int startNumber, int stopNumber, int parallelThreads)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = parallelThreads
            };
            List<Request> requests = new List<Request>();
            Parallel.For(startNumber, stopNumber, options, i =>
            {
                List<JObject> changeLogs = new List<JObject>();
                // The changelog will not return more than 100 entries, so we have to make the call twice
                changeLogs.Add(JsonConvert.DeserializeObject<JObject>(GetRest($"{url}/rest/api/3/issue/{workitemPrefix}-{i}/changelog").Content));
                changeLogs.Add(JsonConvert.DeserializeObject<JObject>(GetRest($"{url}/rest/api/3/issue/{workitemPrefix}-{i}/changelog?startAt=100").Content));
                var issueRequestResponse = JsonConvert.DeserializeObject<JObject>(GetRest($"{url}/rest/api/3/issue/{workitemPrefix}-{i}").Content);
                requests.Add(new Request
                {
                    id = i,
                    issueChangeLogRequestResponses = changeLogs.ToArray(),
                    issueRequestResponse = issueRequestResponse
                });
                Console.WriteLine($"Finished rest call for record: {i}");
            });
            return requests;
        }

        private static IRestResponse GetRest(String uri)
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", auth);
            request.AddCookie("atlassian.xsrf.token", token);
return client.Execute(request);

        }
    }
}
