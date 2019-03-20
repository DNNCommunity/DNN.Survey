# DNN.Survey
The DNN Survey module allows you to create surveys and quizes on your DNN (formerly known as DotNetNuke) web site.
* Answers can be single-choice, multiple-choice or free text
* Result charts (created with ChartJS)
* Quiz questions can be statistical, that means they have no correct answer. This is useful for questions about the gender, age group etc. in a quiz and allows analysis by these items.
* To be able to identify which answers come from the same user (even in a survey that allows anonymous participation) there is a random GUID in the survey results which is created when the survey is submitted.
## System requirements
* [DNN Platform version 09.02.02 or higher](https://github.com/dnnsoftware/Dnn.Platform/releases/tag/v9.2.2)
* [DNN JavaScript Library for Chart.js v2.7.3](https://github.com/EngageSoftware/DNN-JavaScript-Libraries/releases/tag/chart.js_2.7.3)
* SQL Server 2016 (any edition, also Express)
