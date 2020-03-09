using ReadData.Models;
using ReadData.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ReadData
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sr = new StreamWriter($"{Environment.CurrentDirectory}/my.csv"))
            using (var csv = new CsvHelper.CsvWriter(sr, CultureInfo.InvariantCulture))
            {
                int start = 4200;
                int stop = 6400;
                int parallelThreads = 50;
                int failCounter = 0;
                foreach (var request in JiraHttpRequest.getWorkItemObjects(start, stop, parallelThreads))
                {
                    try
                    {
                        csv.WriteRecords(FlattenItems(request));
                        Console.WriteLine($"Finished writing POB-{request.id}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to write POB-{request.id} " + e.ToString());
                        failCounter++;
                    }
                }

                Console.WriteLine($"Failed to write {failCounter} records");
            }
        }

        public static IEnumerable<DataObject> FlattenItems(Request request)
        {
            if (!request.issueRequestResponse["key"].ToString().Contains("POB") 
                || !request.issueRequestResponse["fields"]["issuetype"]["name"].ToString().Contains("Story"))
            {
                throw new Exception("This is not an OLB story");
            }
            var transitions = new List<Transition>();
            lock (transitions)
                foreach (var requestResponse in request.issueChangeLogRequestResponses)
                {
                    var values = requestResponse["values"].Where(v => v["items"].Any(i => i.Value<string>("field") == "status"));
                    foreach (var value in values)
                    {
                        foreach (var item in value["items"])
                        {
                            transitions.Add(new Transition
                            {
                                created = value.Value<DateTime>("created"),
                                toString = item.Value<string>("to"),
                                fromString = item.Value<string>("from")
                            });
                        }
                    }
                }
            Issue issue = new Issue
            {
                id = (string)request.issueRequestResponse["key"],
                type = (string)request.issueRequestResponse["fields"]["issuetype"]["name"],
                storyPoints = (string)request.issueRequestResponse["fields"]["customfield_10024"],
                transitions = transitions.ToArray()

            };
            TransitionHelper transitionHelper = new TransitionHelper(issue);
            yield return new DataObject
            {
                id = issue.id,
                storyPoints = issue.storyPoints,
                type = issue.type,
                qa_total_done_time = transitionHelper.GetTotalQAHours(),
                in_qa_time = transitionHelper.getInQATime(),
                dev_total_done_time = transitionHelper.getTotalDevHours(),
                qa_sitting_time = transitionHelper.getQASittingTime(),
                in_dev_time = transitionHelper.getInDevTime(),
                dev_sitting_time = transitionHelper.getDevSittingTime(),
                time_to_done = transitionHelper.getTotalDevelopmentTime(),
                doneDate = transitionHelper.getDoneDate()
            };
        }
    }
}
