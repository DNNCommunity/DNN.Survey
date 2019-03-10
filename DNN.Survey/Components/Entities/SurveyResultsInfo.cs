using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNN.Modules.Survey.Components.Entities
{
   public class SurveyResultsInfo
   {
      public int SurveyResultID { get; set; }
      public int SurveyOptionID { get; set; }
      public int? UserID { get; set; }
      public string IPAddress { get; set; }
      public string TextAnswer { get; set; }
      public DateTime CreatedDate { get; set; }
      [IgnoreColumn]
      public bool IsCorrect { get; set; }
   }
}