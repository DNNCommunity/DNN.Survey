using DNN.Modules.Survey.Components.Entities;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace DNN.Modules.Survey.Components.Controllers
{
   public class SurveyOptionsController
   {
      #region Data Access
      public SurveyOptionsInfo Get(int surveyOptionID)
      {
         SurveyOptionsInfo surveyOption;
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveyOptionsInfo>();
            surveyOption = rep.GetById(surveyOptionID);
         }
         return surveyOption;
      }

      public List<SurveyOptionsInfo> GetAll(int surveyID)
      {
         IEnumerable<SurveyOptionsInfo> surveyOptions;
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveyOptionsInfo>();
            surveyOptions = rep.Get(surveyID);
         }
         return surveyOptions.OrderBy(so => so.ViewOrder).ToList<SurveyOptionsInfo>();
      }
      #endregion
   }
}