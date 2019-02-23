using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace DNN.Modules.Survey.Components.Entities
{
   [TableName("SurveyOptions")]
   [PrimaryKey("SurveyOptionID", AutoIncrement = true)]
   [Scope("SurveyID")]
   [Cacheable("SurveyOptions", CacheItemPriority.Normal, 20)]
   public class SurveyOptionsInfo
   {
      public int SurveyOptionID { get; set; }
      public int SurveyID { get; set; }
      public int ViewOrder { get; set; }
      public string OptionName { get; set; }
      public int Votes { get; set; }
      public bool IsCorrect { get; set; }
      public DateTime CreatedDate { get; set; }
      public int CreatedByUserID { get; set; }
      public DateTime? LastModifiedDate { get; set; }
      public int? LastModifiedByUserID { get; set; }
   }
}