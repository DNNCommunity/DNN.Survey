# DNN.Survey

[![Build Status](https://dev.azure.com/DNNCommunity/Community%20Modules/_apis/build/status/DNNCommunity.DNN.Survey?branchName=develop)](https://dev.azure.com/DNNCommunity/Community%20Modules/_build/latest?definitionId=7&branchName=develop)

The DNN Survey module allows you to create surveys and quizes on your DNN (formerly known as DotNetNuke) web site.
* Answers can be single-choice, multiple-choice or free text
* Result charts (created with ChartJS)
* Quiz questions can be statistical, that means they have no correct answer. This is useful for questions about the gender, age group etc. in a quiz and allows analysis by these items.
* To be able to identify which answers come from the same user (even in a survey that allows anonymous participation) there is a random GUID in the survey results which is created when the survey is submitted.
* Survey results can be downloaded as a CSV file to be processed with any reporting tool of your choice. To give maximum flexibility, the separator and delimiter can be set in the module settings.
## System requirements
* .Net Framework 4.7.2 or higher
* [DNN Platform version 09.04.00 or higher](https://github.com/dnnsoftware/Dnn.Platform/releases/tag/v9.4.0)
## More information
This project is documented on [dnn-survey.readme.io](https://dnn-survey.readme.io)
