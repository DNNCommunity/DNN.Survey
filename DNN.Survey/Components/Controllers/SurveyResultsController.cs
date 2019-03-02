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
   public class SurveyResultsController
   {
      #region Data Access
      public List<SurveyResultsInfo> GetAll(int moduleID)
      {
         IEnumerable<SurveyResultsInfo> surveyResults;
         using (IDataContext ctx = DataContext.Instance())
         {
            surveyResults = ctx.ExecuteQuery<SurveyResultsInfo>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResults_GetAll",
               moduleID);
         }
         return surveyResults.ToList<SurveyResultsInfo>();
      }

      public void Add(List<SurveyResultsInfo> surveyResults, TrackingMethod trackingMethod)
      {
         using (IDataContext ctx = DataContext.Instance())
         {
            ctx.Execute(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResults_Add",
               Convert.ToInt32(trackingMethod),
               XmlDataProvider.SurveyResultsToXml(surveyResults)
            );
         }
         DataCache.ClearCache("Surveys");
         DataCache.ClearCache("SurveyOptions");
         DataCache.ClearCache("SurveyResults");
      }

      public void DropAll(int moduleID)
      {
         using (IDataContext ctx = DataContext.Instance())
         {
            ctx.Execute(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResultData_Delete",
               moduleID);
         }
         DataCache.ClearCache("Surveys");
         DataCache.ClearCache("SurveyOptions");
         DataCache.ClearCache("SurveyResults");
      }

      public List<SurveyResultInfo> Get(int surveyID)
      {
         IEnumerable<SurveyResultInfo> surveyResult;
         using (IDataContext ctx = DataContext.Instance())
         {
            surveyResult = ctx.ExecuteQuery<SurveyResultInfo>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResults_Get",
               surveyID);
         }
         return surveyResult.ToList<SurveyResultInfo>();
      }
      #endregion
   }
}