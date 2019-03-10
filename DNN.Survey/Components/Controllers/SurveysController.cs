using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Components.Providers;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

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
                  ((survey.RepeatDirection == null) || (survey.RepeatDirection == RepeatDirection.Horizontal) ? (int?)null : Convert.ToInt32(survey.RepeatDirection.Value)),
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