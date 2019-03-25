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

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
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