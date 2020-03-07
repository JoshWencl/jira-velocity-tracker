using ReadData.Models;
using System;
using System.Linq;

namespace ReadData.Utilities
{
    public class TransitionHelper
    {
        Issue issue;
        public TransitionHelper(Issue issue)
        {
            this.issue = issue;
        }
         
        public double GetTotalQAHours()
        {
            try
            {
                return getTimeBetweenCodes(TransitionCodes.ReadyForQA, TransitionCodes.QADone, issue);

            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Sequence contains no matching element"))
                {
                    return getTimeBetweenCodes(TransitionCodes.InQA, TransitionCodes.QADone, issue);
                }
                else
                {
                    throw e;
                }
            }
        }

        public double getInQATime()
        {
            return getTimeBetweenCodes(TransitionCodes.InQA, TransitionCodes.QADone, issue);
        }

        public double getQASittingTime()
        {
            try
            {
                return getTimeBetweenCodes(TransitionCodes.ReadyForQA, TransitionCodes.InQA, issue);
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Sequence contains no matching element"))
                {
                    return 0;
                }
                else
                {
                    throw e;
                }
            }
        }

        public double getDevSittingTime()
        {
            return getTimeBetweenCodes(TransitionCodes.ReadyForDev, TransitionCodes.InDev, issue);
        }

        public double getInDevTime()
        {
            try
            {
                return getTimeBetweenCodes(TransitionCodes.InDev, TransitionCodes.ReadyForQA, issue);
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Sequence contains no matching element"))
                {
                    return getTimeBetweenCodes(TransitionCodes.InDev, TransitionCodes.InQA, issue);
                }
                else
                {
                    throw e;
                }
            }
        }

        public double getTotalDevHours()
        {
            try
            {
                return getTimeBetweenCodes(TransitionCodes.ReadyForDev, TransitionCodes.ReadyForQA, issue);
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Sequence contains no matching element"))
                {
                    return getTimeBetweenCodes(TransitionCodes.ReadyForDev, TransitionCodes.InQA, issue);
                }
                else
                {
                    throw e;
                }
            }
        }

        public string getDoneDate()
        {
           return issue.transitions.Last(j => TransitionCodes.QADone.Any(k => k.Equals(j.toString))).created;
        }

        private bool isUserStory()
        {
            return (string.IsNullOrEmpty(issue.storyPoints) || issue.type != "Story");
        }

        private bool isWorkflowComplete()
        {
            return (issue.transitions.Any(j => j.toString.Any(x => TransitionCodes.InQA.Equals(x) && j.fromString.Any(x => TransitionCodes.QADone.Equals(x)))));
        }

        private double getTimeBetweenCodes(string[] start, string[] stop, Issue story)
        {
            if (isUserStory())
            {
                throw new Exception("Work item is not a story");
            }
            if (isWorkflowComplete())
            {
                throw new Exception("Story is not 'complete'");
            }
            string startTime = issue.transitions.First(j => start.Any(k=> k.Equals(j.toString))).created;
            string endTime = issue.transitions.Last(j => stop.Any(k => k.Equals(j.toString))).created;
            DateTime startDate = DateTime.Parse(startTime);
            DateTime endDate = DateTime.Parse(endTime);
            var timeSpan = (endDate - startDate);
            int weekends = 0;
            for (int i = 0; i < timeSpan.Days; i++)
            {
                if (startDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday || startDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                {
                    weekends++;
                }
            }
            return (endDate - startDate).TotalHours - (weekends * 24);
        }
    }
}
