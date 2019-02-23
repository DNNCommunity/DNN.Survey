using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNN.Modules.Survey.Components
{
   public enum TrackingMethod
   {
      Cookie = 0,
      User = 1
   }

   public enum QuestionType
   {
      RadioButtons = 0,
      CheckBoxes = 1,
      Text = 2
   }

   public class Base
   {
      public const string DEFAULT_SURVEY_RESULTS_TEMPLATE = "[SURVEY_OPTION_NAME]&nbsp;([SURVEY_OPTION_VOTES])&nbsp;<img src=\"[SURVEY_OPTION_IMAGEPATH]/red.gif\" width=\"[SURVEY_OPTION_GRAPH_WIDTH]\" border=\"0\" height=\"15\" alt=\"\" />&nbsp;[SURVEY_OPTION_PERCENTAGE]%<br />";
   }

   public class ModuleSecurity
   {
      public const string VIEW_RESULTS_PERMISSION = "VIEW_RESULTS_PERMISSION";
   }

}