using DNN.Modules.Survey.Components.Entities;
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
                  userID,
                  answersXml
               }
            );
         }
         DataCache.ClearCache("Surveys");
         DataCache.ClearCache("SurveyOptions");
         return id;
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