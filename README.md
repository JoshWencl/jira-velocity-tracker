# jira-velocity-tracker

This console app can be used to scrape data from Jira to gather velocity metrics.

I used this to gather information on user stories to show how QA has increased velocity.

The important metrics were

1. What was the lead time for stories sitting waiting for a QA to pick it up.
2. How long did stories spend in QA being tested.  If you're making process changes, then this is the metric you want.  You want automation to decrease work, not increase work.

Since the status codes for some of these transitions changed, the transition codes are setup as arrays, so plug in the status codes for each transition.
