/*
 * DNN® - https://www.dnnsoftware.com
 * Copyright (c) 2002-2019
 * by DNN Corp.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions
 * of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

using System;

namespace DNN.Modules.Survey.Components
{
   public enum QuestionType
   {
      RadioButtons = 0,
      CheckBoxes = 1,
      Text = 2
   }

   public enum ChartType
   {
      Bar = 0,
      HorizontalBar = 1,
      Pie = 2,
      Doughnut = 3,
      List = 4,
      Table = 5
   }

   public enum UseCaptcha
   {
      Never = 0,
      UnauthorizedUsersOnly = 1,
      Always = 2
   }

   public enum SurveyType
   {
      Survey = 0,
      Quiz = 1
   }

   public enum Separator
   {
      SemiColon = 0,
      Comma = 1,
      Space = 2,
      Tab = 3
   }

   public enum TextQualifier
   {
      None = 0,
      DoubleQuote = 1,
      SingleQuote = 2
   }

   public static class Base
   {
      public const string DEFAULT_SURVEY_RESULTS_TEMPLATE = "[SURVEY_OPTION_NAME]&nbsp;([SURVEY_OPTION_VOTES])&nbsp;<img src=\"[SURVEY_OPTION_IMAGEPATH]/red.gif\" width=\"[SURVEY_OPTION_GRAPH_WIDTH]\" border=\"0\" height=\"15\" alt=\"\" />&nbsp;[SURVEY_OPTION_PERCENTAGE]%<br />";

      private static string[] defaultSurveyChartColors
      {
         get
         {
            return new string[] { "68,114,196","237,125,49","165,165,165","255,192,0","91,155,213","112,173,71","38,68,120","158,72,14","99,99,99","153,115,0","37,94,145","67,104,43","105,142,208","241,151,90","183,183,183","255,205,51","124,175,221","140,193,104","51,90,161","210,96,18","132,132,132","204,154,0","50,125,194","90,138,57","143,170,220","244,177,131" };
         }
      }

      public static string GetColor(int index, bool isBackGround)
      {
         return string.Format("rgba({0},{1})", defaultSurveyChartColors[index % defaultSurveyChartColors.Length], (isBackGround ? 0.2 : 1));
      }

      public static string ToChartJSChartType(ChartType chartType)
      {
         string chartTypeName = Enum.GetName(typeof(ChartType), chartType);
         // lower first character
         chartTypeName = string.Format("{0}{1}", chartTypeName.Substring(0, 1).ToLower(), chartTypeName.Substring(1));
         return chartTypeName;
      }
   }

   public class ModuleSecurity
   {
      public const string PERMISSION_CODE = "DNN_SURVEY";
      public const string PARTICIPATE_PERMISSION = "PARTICIPATE_PERMISSION";
      public const string VIEW_RESULTS_PERMISSION = "VIEW_RESULTS_PERMISSION";
      public const string EDIT_PERMISSION = "EDIT";
   }

}