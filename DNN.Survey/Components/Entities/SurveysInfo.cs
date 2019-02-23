using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Modules;
using System;
using System.Data;
using System.Web.Caching;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Components.Entities
{
   [TableName("Surveys")]
   [PrimaryKey("SurveyID", AutoIncrement = true)]
   [Scope("ModuleID")]
   [Cacheable("Surveys", CacheItemPriority.Normal, 20)]
   public class SurveysInfo
   {
      public int SurveyID { get; set; }
      public int ModuleID { get; set; }
      public string Question { get; set; }
      public int ViewOrder { get; set; }
      public QuestionType OptionType { get; set; }
      public RepeatDirection? RepeatDirection { get; set; }
      public int? RepeatColumns { get; set; }
      public DateTime CreatedDate { get; set; }
      public int CreatedByUserID { get; set; }
      public DateTime? LastModifiedDate { get; set; }
      public int? LastModifiedByUserID { get; set; }
      [IgnoreColumn]
      public string SurveyOptionsXml { get; set; }

      //public void Fill(IDataReader dr)
      //{
      //   SurveyID = Convert.ToInt32(dr["SurveyOptionID"]);
      //   ModuleID = Convert.ToInt32(dr["ModuleID"]);
      //   Question = Convert.ToString(dr["Question"]);
      //   ViewOrder = Convert.ToInt32(dr["ViewOrder"]);
      //   OptionType = (QuestionType)Convert.ToInt32(dr["OptionType"]);
      //   CreatedDate = Convert.ToDateTime(dr["CreatedDate"]);
      //   CreatedByUserID = Convert.ToInt32(dr["CreatedByUserID"]);
      //   LastModifiedDate = Convert.ToDateTime(Null.SetNull(dr["LastModifiedDate"], LastModifiedDate));
      //   LastModifiedByUserID = Convert.ToInt32(Null.SetNull(dr["LastModifiedByUserID"], LastModifiedByUserID));
      //}

      //public int KeyID
      //{
      //   get { return SurveyID; }
      //   set { SurveyID = value; }
      //}
   }
}