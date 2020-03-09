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
            calcTransitionTimes();
        }

        public double GetTotalQAHours()
        {
            try
            {
                return getTimeBetweenCodes(TransitionCodes.ReadyForQA, TransitionCodes.QADone);

            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Sequence contains no matching element"))
                {
                    return getTimeBetweenCodes(TransitionCodes.InQA, TransitionCodes.QADone);
                }
                else
                {
                    throw e;
                }
            }
        }

        public double getQASittingTime()
        {
            return getTimeSpentInCode(TransitionCodes.ReadyForQA);
        }

        public double getInQATime()
        {
            return getTimeSpentInCode(TransitionCodes.InQA);
        }

        public double getDevSittingTime()
        {
            return getTimeSpentInCode(TransitionCodes.ReadyForDev);
        }

        public double getInDevTime()
        {
            return getTimeSpentInCode(TransitionCodes.InDev);
        }

        public double getTotalDevHours()
        {
            try
            {
                return getTimeBetweenCodes(TransitionCodes.ReadyForDev, TransitionCodes.ReadyForQA);
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Sequence contains no matching element"))
                {
                    return getTimeBetweenCodes(TransitionCodes.ReadyForDev, TransitionCodes.InQA);
                }
                else
                {
                    throw e;
                }
            }
        }

        public double getTotalDevelopmentTime()
        {
            return getTimeBetweenCodes(TransitionCodes.ReadyForDev, TransitionCodes.QADone);
        }

        public DateTime getDoneDate()
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

        private double getTimeBetweenCodes(string[] start, string[] stop)
        {
            if (isUserStory())
            {
                throw new Exception("Work item is not a story");
            }
            if (isWorkflowComplete())
            {
                throw new Exception("Story is not 'complete'");
            }
            DateTime startDate = issue.transitions.First(j => start.Any(k => k.Equals(j.toString))).created;
            DateTime endDate = issue.transitions.Last(j => stop.Any(k => k.Equals(j.toString))).created;
            int weekends = getWorkDaysBetweenDates(startDate, endDate);
            return (endDate - startDate).TotalHours - (weekends * 24);
        }

        private double getTimeSpentInCode(string[] start)
        {
            double hoursSpent = 0;
            foreach (var transition in issue.transitions.Where(t => start.Any(k => k.Equals(t.toString))))
            {
                int weekends = getWorkDaysBetweenDates(transition.created, transition.created.Add(transition.timeInStatus));
                hoursSpent += transition.timeInStatus.TotalHours - (weekends * 24);
            }
            return hoursSpent;
        }

        private void calcTransitionTimes()
        {
            var list = issue.transitions.OrderBy(date => date.created).ToList();
            // For every status get the time difference between the next status
            for (int i = 0; i < list.Count - 1; i++)
            {
                list[i].timeInStatus = (list[i + 1].created - list[i].created);
            }
            issue.transitions = list.ToArray();
        }

        private int getWorkDaysBetweenDates(DateTime start, DateTime end)
        {
            if (start > end)
                throw new Exception ("start date is greater than end date");

            var timeSpan = (end - start);
            int weekends = 0;
            for (int i = 0; i < timeSpan.Days; i++)
            {
                if (start.AddDays(i).DayOfWeek == DayOfWeek.Saturday || start.AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                {
                    weekends++;
                }
            }
            return weekends;
        }
    }
}
