using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace DNN.Modules.Survey.Components.Entities
{
   [Cacheable("Surveys", CacheItemPriority.Normal, 20)]
   public class SurveysExportInfo
   {
      public int SurveyID { get; set; }
      public int ModuleID { get; set; }
      public string Question { get; set; }
      public int SurveyViewOrder { get; set; }
      public QuestionType OptionType { get; set; }
      public bool? IsStatistical { get; set; }
      public int SurveyOptionViewOrder { get; set; }
      public string OptionName { get; set; }
      public int Votes { get; set; }
      public bool IsCorrect { get; set; }
      public int? UserID { get; set; }
      public string IPAddress { get; set; }
      public string TextAnswer { get; set; }
      public Guid ResultUserID { get; set; }
      public DateTime CreatedDate { get; set; }
   }
}