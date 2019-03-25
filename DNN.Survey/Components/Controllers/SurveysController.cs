/*
 * DNN® - https://www.dnnsoftware.com
 * Copyright (c) 2002-2019
 * by DNN Corp.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Components.Providers;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DNN.Modules.Survey.Components.Controllers
{
   public class SurveysController
   {
      #region Data Access
      public SurveysInfo Get(int surveyID)
      {
         SurveysInfo survey;
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveysInfo>();
            survey = rep.GetById(surveyID);
         }
         return survey;
      }

      public List<SurveysInfo> GetAll(int moduleID)
      {
         IEnumerable<SurveysInfo> surveys;
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveysInfo>();
            surveys = rep.Get(moduleID);
         }
         return surveys.OrderBy(s => s.ViewOrder).ToList<SurveysInfo>();
      }

      public bool HasVoted(int moduleID, int userID)
      {
         bool hasVoted;
         using (IDataContext ctx = DataContext.Instance())
         {
            hasVoted = ctx.ExecuteScalar<bool>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}Surveys_HasVoted",
               moduleID,
               userID);
         }
         return hasVoted;
      }

      public int AddOrChange(SurveysInfo survey, string answersXml, int userID)
      {
         int id;
         using (IDataContext ctx = DataContext.Instance())
         {
            id = ctx.ExecuteScalar<int>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}Surveys_AddOrChange",
               new object[] {
                  survey.SurveyID,
                  survey.ModuleID,
                  survey.Question,
                  Convert.ToInt32(survey.OptionType),
                  ((survey.IsStatistical == null) ? (bool?)null : survey.IsStatistical.Value),
                  Convert.ToInt32(survey.RepeatDirection),
                  ((survey.RepeatColumns == null) || (survey.RepeatColumns <= 1) ? (int?)null : survey.RepeatColumns.Value),
                  ((survey.NumberOfRows == null) || (survey.NumberOfRows <= 1) ? (int?)null : survey.NumberOfRows.Value),
                  Convert.ToInt32(survey.ChartType),
                  userID,
                  answersXml
               }
            );
         }
         DataCache.ClearCache("Surveys");
         DataCache.ClearCache("SurveyOptions");
         return id;
      }

      public void Sort(List<SurveysInfo> surveys)
      {
         using (IDataContext ctx = DataContext.Instance())
         {
            ctx.Execute(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}Surveys_Sort",
               new object[] { XmlDataProvider.SurveysToXml(surveys) }
            );
         }
         DataCache.ClearCache("Surveys");
      }

      public void Drop(SurveysInfo survey)
      {
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveysInfo>();
            rep.Delete(survey);
         }
      }
      #endregion
   }
}