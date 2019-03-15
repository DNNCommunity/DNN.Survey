using DNN.Modules.Survey.Components.Entities;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DNN.Modules.Survey.Components.Controllers
{
   public class SurveysExportController
   {
      public List<SurveysExportInfo> GetAll(int moduleID)
      {
         IEnumerable<SurveysExportInfo> surveys;
         using (IDataContext ctx = DataContext.Instance())
         {
            surveys = ctx.ExecuteQuery<SurveysExportInfo>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}Surveys_CsvExport",
               moduleID);
         }
         return surveys.OrderBy(s => s.SurveyViewOrder).ThenBy(s => s.SurveyOptionViewOrder).ToList<SurveysExportInfo>();
      }
   }
}