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
      public bool? IsStatistical { get; set; }
      public RepeatDirection RepeatDirection { get; set; }
      public int? RepeatColumns { get; set; }
      public int? NumberOfRows { get; set; }
      public ChartType ChartType { get; set; }
      public DateTime CreatedDate { get; set; }
      public int CreatedByUserID { get; set; }
      public DateTime? LastModifiedDate { get; set; }
      public int? LastModifiedByUserID { get; set; }
      [IgnoreColumn]
      public string SurveyOptionsXml { get; set; }
   }
}