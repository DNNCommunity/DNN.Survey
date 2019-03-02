using DNN.Modules.Survey.Components.Controllers;
using DNN.Modules.Survey.Components.Entities;
using DotNetNuke.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace DNN.Modules.Survey.Components.Providers
{
   public static class XmlDataProvider
   {
      #region Private Properties
      private static SurveyOptionsController _surveyOptionsController = null;

      private static SurveyOptionsController SurveyOptionsController
      {
         get
         {
            if (_surveyOptionsController == null)
               _surveyOptionsController = new SurveyOptionsController();
            return _surveyOptionsController;
         }
      }
      #endregion

      #region Surveys
      public static string SurveyToXml(SurveysInfo survey)
      {
         return SurveyToXml(survey, false);
      }

      public static string SurveyToXml(SurveysInfo survey, bool forExport)
      {
         StringBuilder surveyBuilder = new StringBuilder();
         if (survey == null)
         {
            surveyBuilder.Append("<Survey />");
         }
         else
         {
            surveyBuilder.Append("<Survey>");
            surveyBuilder.Append(String.Format("<SurveyID>{0}</SurveyID>", survey.SurveyID));
            surveyBuilder.Append(String.Format("<ModuleID>{0}</ModuleID>", survey.ModuleID));
            surveyBuilder.Append(String.Format("<Question><![CDATA[{0}]]></Question>", survey.Question));
            surveyBuilder.Append(String.Format("<ViewOrder>{0}</ViewOrder>", survey.ViewOrder));
            surveyBuilder.Append(String.Format("<OptionType>{0}</OptionType>", Convert.ToInt32(survey.OptionType)));
            if ((survey.RepeatDirection == null) || (survey.RepeatDirection == RepeatDirection.Horizontal))
            {
               surveyBuilder.Append("<RepeatDirection />");
            }
            else
            {
               surveyBuilder.Append(String.Format("<RepeatDirection>{0}</RepeatDirection>", Convert.ToInt32(survey.RepeatDirection.Value)));
            }
            if ((survey.RepeatColumns == null) || (survey.RepeatColumns <= 1))
            {
               surveyBuilder.Append("<RepeatColumns />");
            }
            else
            {
               surveyBuilder.Append(String.Format("<RepeatColumns>{0}</RepeatColumns>", survey.RepeatColumns.Value));
            }
            if ((survey.NumberOfRows == null) || (survey.NumberOfRows <= 1))
            {
               surveyBuilder.Append("<NumberOfRows />");
            }
            else
            {
               surveyBuilder.Append(String.Format("<NumberOfRows>{0}</NumberOfRows>", survey.NumberOfRows.Value));
            }
            surveyBuilder.Append(String.Format("<CreatedDate>{0:yyyy-MM-dd hh:mm:ss}</CreatedDate>", survey.CreatedDate));
            surveyBuilder.Append(String.Format("<CreatedByUserID>{0}</CreatedByUserID>", survey.CreatedByUserID));
            if (survey.LastModifiedByUserID == null)
            {
               surveyBuilder.Append("<LastModifiedByUserID />");
            }
            else
            {
               surveyBuilder.Append(String.Format("<LastModifiedByUserID>{0}</LastModifiedByUserID>", survey.LastModifiedByUserID.Value));
            }
            if (survey.LastModifiedDate == null)
            {
               surveyBuilder.Append("<LastModifiedDate />");
            }
            else
            {
               surveyBuilder.Append(String.Format("<LastModifiedDate>{0:yyyy-MM-dd hh:mm:ss}</LastModifiedDate>", survey.LastModifiedDate.Value));
            }
            if (forExport)
            {
               surveyBuilder.Append(SurveyOptionsToXml(SurveyOptionsController.GetAll(survey.SurveyID)));
            }
            surveyBuilder.Append("</Survey>");
         }
         return surveyBuilder.ToString();
      }

      public static string SurveysToXml(List<SurveysInfo> surveys)
      {
         return SurveysToXml(surveys, false);
      }

      public static string SurveysToXml(List<SurveysInfo> surveys, bool forExport)
      {
         StringBuilder surveysBuilder = new StringBuilder();
         if (surveys == null)
         {
            surveysBuilder.Append("<Surveys />");
         }
         else
         {
            surveysBuilder.Append("<Surveys>");
            foreach (SurveysInfo si in surveys)
            {
               surveysBuilder.Append(SurveyToXml(si, forExport));
            }
            surveysBuilder.Append("</Surveys>");
         }
         return surveysBuilder.ToString();
      }

      public static SurveysInfo SurveyFromXml(string surveyXml)
      {
         SurveysInfo survey = null;
         if ((!(String.IsNullOrEmpty(surveyXml))) && (surveyXml != "<Survey />"))
         {
            survey = new SurveysInfo();
            XmlNode surveyXmlNode = Globals.GetContent(surveyXml, "Survey");
            survey.SurveyID = Convert.ToInt32(surveyXmlNode.SelectSingleNode("SurveyID").InnerText);
            survey.ModuleID = Convert.ToInt32(surveyXmlNode.SelectSingleNode("ModuleID").InnerText);
            survey.Question = surveyXmlNode.SelectSingleNode("Question").InnerText;
            survey.ViewOrder = Convert.ToInt32(surveyXmlNode.SelectSingleNode("ViewOrder").InnerText);
            survey.OptionType = (QuestionType)Convert.ToInt32(surveyXmlNode.SelectSingleNode("OptionType").InnerText);
            if ((surveyXmlNode.SelectSingleNode("RepeatDirection") != null) && (!(String.IsNullOrEmpty(surveyXmlNode.SelectSingleNode("RepeatDirection").InnerText))))
            {
               survey.RepeatDirection = (RepeatDirection)Convert.ToInt32(surveyXmlNode.SelectSingleNode("RepeatDirection").InnerText);
            }
            if ((surveyXmlNode.SelectSingleNode("RepeatColumns") != null) && (!(String.IsNullOrEmpty(surveyXmlNode.SelectSingleNode("RepeatColumns").InnerText))))
            {
               survey.RepeatColumns = Convert.ToInt32(surveyXmlNode.SelectSingleNode("RepeatColumns").InnerText);
            }
            if ((surveyXmlNode.SelectSingleNode("NumberOfRows") != null) && (!(String.IsNullOrEmpty(surveyXmlNode.SelectSingleNode("NumberOfRows").InnerText))))
            {
               survey.NumberOfRows = Convert.ToInt32(surveyXmlNode.SelectSingleNode("NumberOfRows").InnerText);
            }
            survey.CreatedDate = Convert.ToDateTime(surveyXmlNode.SelectSingleNode("CreatedDate").InnerText);
            survey.CreatedByUserID = Convert.ToInt32(surveyXmlNode.SelectSingleNode("CreatedByUserID").InnerText);
            if ((surveyXmlNode.SelectSingleNode("LastModifiedDate") != null) && (!(String.IsNullOrEmpty(surveyXmlNode.SelectSingleNode("LastModifiedDate").InnerText))))
            {
               survey.LastModifiedDate = Convert.ToDateTime(surveyXmlNode.SelectSingleNode("LastModifiedDate").InnerText);
            }
            if ((surveyXmlNode.SelectSingleNode("LastModifiedByUserID") != null) && (!(String.IsNullOrEmpty(surveyXmlNode.SelectSingleNode("LastModifiedByUserID").InnerText))))
            {
               survey.LastModifiedByUserID = Convert.ToInt32(surveyXmlNode.SelectSingleNode("LastModifiedByUserID").InnerText);
            }
            // When it comes from an export...
            if (surveyXmlNode.SelectSingleNode("SurveyOptions") != null)
            {
               survey.SurveyOptionsXml = surveyXmlNode.SelectSingleNode("SurveyOptions").OuterXml;
            }
         }
         return survey;
      }

      public static List<SurveysInfo> SurveysFromXml(string surveysXml)
      {
         List<SurveysInfo> surveys = null;
         if ((!(String.IsNullOrEmpty(surveysXml))) && (surveysXml != "<Surveys />"))
         {
            surveys = new List<SurveysInfo>();
            XmlNode surveysXmlNode = Globals.GetContent(surveysXml, "Surveys");

            foreach (XmlNode surveyXmlNode in surveysXmlNode)
            {
               SurveysInfo survey = SurveyFromXml(surveyXmlNode.OuterXml);
               if (survey != null)
                  surveys.Add(survey);
            }
         }
         return surveys;
      }
      #endregion

      #region SurveyOptions
      public static string SurveyOptionToXml(SurveyOptionsInfo surveyOption)
      {
         StringBuilder surveyOptionBuilder = new StringBuilder();
         if (surveyOption == null)
         {
            surveyOptionBuilder.Append("<SurveyOption />");
         }
         else
         {
            surveyOptionBuilder.Append("<SurveyOption>");
            surveyOptionBuilder.Append(String.Format("<SurveyOptionID>{0}</SurveyOptionID>", surveyOption.SurveyOptionID));
            surveyOptionBuilder.Append(String.Format("<ViewOrder>{0}</ViewOrder>", surveyOption.ViewOrder));
            surveyOptionBuilder.Append(String.Format("<OptionName><![CDATA[{0}]]></OptionName>", surveyOption.OptionName));
            surveyOptionBuilder.Append(String.Format("<Votes>{0}</Votes>", surveyOption.Votes));
            surveyOptionBuilder.Append(String.Format("<IsCorrect>{0}</IsCorrect>", surveyOption.IsCorrect));
            surveyOptionBuilder.Append(String.Format("<CreatedDate>{0:yyyy-MM-dd hh:mm:ss}</CreatedDate>", surveyOption.CreatedDate));
            surveyOptionBuilder.Append(String.Format("<CreatedByUserID>{0}</CreatedByUserID>", surveyOption.CreatedByUserID));
            if (surveyOption.LastModifiedDate == null)
            {
               surveyOptionBuilder.Append("<LastModifiedDate />");
            }
            else
            {
               surveyOptionBuilder.Append(String.Format("<LastModifiedDate>{0:yyyy-MM-dd hh:mm:ss}</LastModifiedDate>", surveyOption.LastModifiedDate.Value));
            }
            if (surveyOption.LastModifiedByUserID == null)
            {
               surveyOptionBuilder.Append("<LastModifiedByUserID />");
            }
            else
            {
               surveyOptionBuilder.Append(String.Format("<LastModifiedByUserID>{0}</LastModifiedByUserID>", surveyOption.LastModifiedByUserID.Value));
            }
            surveyOptionBuilder.Append("</SurveyOption>");
         }
         return surveyOptionBuilder.ToString();
      }

      public static string SurveyOptionsToXml(List<SurveyOptionsInfo> surveyOptions)
      {
         StringBuilder surveyOptionsBuilder = new StringBuilder();
         if (surveyOptions == null)
         {
            surveyOptionsBuilder.Append("<SurveyOptions />");
         }
         else
         {
            surveyOptionsBuilder.Append("<SurveyOptions>");
            foreach (SurveyOptionsInfo soi in surveyOptions)
            {
               surveyOptionsBuilder.Append(SurveyOptionToXml(soi));
            }
            surveyOptionsBuilder.Append("</SurveyOptions>");
         }
         return surveyOptionsBuilder.ToString();
      }

      public static SurveyOptionsInfo SurveyOptionFromXml(string surveyOptionXml)
      {
         SurveyOptionsInfo surveyOption = null;
         if ((!(string.IsNullOrEmpty(surveyOptionXml))) && (surveyOptionXml != "<SurveyOption />"))
         {
            surveyOption = new SurveyOptionsInfo();
            XmlNode surveyOptionXmlNode = Globals.GetContent(surveyOptionXml, "SurveyOption");
            surveyOption.SurveyOptionID = Convert.ToInt32(surveyOptionXmlNode.SelectSingleNode("SurveyOptionID").InnerText);
            surveyOption.ViewOrder = Convert.ToInt32(surveyOptionXmlNode.SelectSingleNode("ViewOrder").InnerText);
            surveyOption.OptionName = surveyOptionXmlNode.SelectSingleNode("OptionName").InnerText;
            surveyOption.Votes = Convert.ToInt32(surveyOptionXmlNode.SelectSingleNode("Votes").InnerText);
            surveyOption.IsCorrect = Convert.ToBoolean(surveyOptionXmlNode.SelectSingleNode("IsCorrect").InnerText);
            surveyOption.CreatedDate = Convert.ToDateTime(surveyOptionXmlNode.SelectSingleNode("CreatedDate").InnerText);
            surveyOption.CreatedByUserID = Convert.ToInt32(surveyOptionXmlNode.SelectSingleNode("CreatedByUserID").InnerText);
            if ((surveyOptionXmlNode.SelectSingleNode("LastModifiedDate") != null) && (!(String.IsNullOrEmpty(surveyOptionXmlNode.SelectSingleNode("LastModifiedDate").InnerText))))
            {
               surveyOption.LastModifiedDate = Convert.ToDateTime(surveyOptionXmlNode.SelectSingleNode("LastModifiedDate").InnerText);
            }
            if ((surveyOptionXmlNode.SelectSingleNode("LastModifiedByUserID") != null) && (!(String.IsNullOrEmpty(surveyOptionXmlNode.SelectSingleNode("LastModifiedByUserID").InnerText))))
            {
               surveyOption.LastModifiedByUserID = Convert.ToInt32(surveyOptionXmlNode.SelectSingleNode("LastModifiedByUserID").InnerText);
            }
         }
         return surveyOption;
      }

      public static List<SurveyOptionsInfo> SurveyOptionsFromXml(string surveyOptionsXml)
      {
         List<SurveyOptionsInfo> surveyOptions = null;
         if ((!(string.IsNullOrEmpty(surveyOptionsXml))) && (surveyOptionsXml != "<SurveyOptions />"))
         {
            surveyOptions = new List<SurveyOptionsInfo>();
            XmlNode surveyOptionsXmlNode = Globals.GetContent(surveyOptionsXml, "SurveyOptions");

            foreach (XmlNode surveyOptionXmlNode in surveyOptionsXmlNode)
            {
               SurveyOptionsInfo surveyOption = SurveyOptionFromXml(surveyOptionXmlNode.OuterXml);
               if (surveyOption != null)
                  surveyOptions.Add(surveyOption);
            }
         }
         return surveyOptions;
      }
      #endregion

      #region SurveyResults
      public static string SurveyResultToXml(SurveyResultsInfo surveyResult)
      {
         StringBuilder surveyResultBuilder = new StringBuilder();
         if (surveyResult == null)
         {
            surveyResultBuilder.Append("<SurveyResult />");
         }
         else
         {
            surveyResultBuilder.Append("<SurveyResult>");
            surveyResultBuilder.Append(String.Format("<SurveyResultID>{0}</SurveyResultID>", surveyResult.SurveyResultID));
            surveyResultBuilder.Append(String.Format("<SurveyOptionID>{0}</SurveyOptionID>", surveyResult.SurveyOptionID));
            if (surveyResult.UserID == null)
            {
               surveyResultBuilder.Append("<UserID />");
            }
            else
            {
               surveyResultBuilder.Append(String.Format("<UserID>{0}</UserID>", surveyResult.UserID.Value));
            }
            if (String.IsNullOrEmpty(surveyResult.IPAddress))
            {
               surveyResultBuilder.Append("<IPAddress />");
            }
            else
            {
               surveyResultBuilder.Append(String.Format("<IPAddress>{0}</IPAddress>", surveyResult.IPAddress));
            }
            if (String.IsNullOrEmpty(surveyResult.TextAnswer))
            {
               surveyResultBuilder.Append("<TextAnswer />");
            }
            else
            {
               surveyResultBuilder.Append(String.Format("<TextAnswer><![CDATA[{0}]]></TextAnswer>", surveyResult.TextAnswer));
            }
            surveyResultBuilder.Append(String.Format("<CreatedDate>{0:yyyy-MM-dd hh:mm:ss}</CreatedDate>", surveyResult.CreatedDate));
            surveyResultBuilder.Append("</SurveyResult>");
         }
         return surveyResultBuilder.ToString();
      }

      public static string SurveyResultsToXml(List<SurveyResultsInfo> surveyResults)
      {
         StringBuilder surveyResultsBuilder = new StringBuilder();
         if (surveyResults == null)
         {
            surveyResultsBuilder.Append("<SurveyResults />");
         }
         else
         {
            surveyResultsBuilder.Append("<SurveyResults>");
            foreach(SurveyResultsInfo sri in surveyResults)
            {
               surveyResultsBuilder.Append(SurveyResultToXml(sri));
            }
            surveyResultsBuilder.Append("</SurveyResults>");
         }
         return surveyResultsBuilder.ToString();
      }

      public static SurveyResultsInfo SurveyResultFromXml(string surveyResultXml)
      {
         SurveyResultsInfo surveyResult = null;
         if ((!(String.IsNullOrEmpty(surveyResultXml))) && (surveyResultXml != "<SurveyResult />"))
         {
            surveyResult = new SurveyResultsInfo();
            XmlNode surveyResultXmlNode = Globals.GetContent(surveyResultXml, "SurveyResult");
            surveyResult.SurveyResultID = Convert.ToInt32(surveyResultXmlNode.SelectSingleNode("SurveyResultID").InnerText);
            surveyResult.SurveyOptionID = Convert.ToInt32(surveyResultXmlNode.SelectSingleNode("SurveyOptionID").InnerText);
            if (surveyResultXmlNode.SelectSingleNode("UserID") != null)
            {
               surveyResult.UserID = Convert.ToInt32(surveyResultXmlNode.SelectSingleNode("UserID").InnerText);
            }
            else
            {
               surveyResult.UserID = 0;
            }
            if (surveyResultXmlNode.SelectSingleNode("IPAddress") != null)
            {
               surveyResult.IPAddress = surveyResultXmlNode.SelectSingleNode("IPAddress").InnerText; 
            }
            else
            {
               surveyResult.IPAddress = String.Empty;
            }
            if (surveyResultXmlNode.SelectSingleNode("TextAnswer") != null)
            {
               surveyResult.TextAnswer = surveyResultXmlNode.SelectSingleNode("TextAnswer").InnerText;
            }
            else
            {
               surveyResult.TextAnswer = String.Empty;
            }
            surveyResult.CreatedDate = Convert.ToDateTime(surveyResultXmlNode.SelectSingleNode("CreatedDate").InnerText);
         }
         return surveyResult;
      }

      public static List<SurveyResultsInfo> SurveyResultsFromXml(string surveyResultsXml)
      {
         List<SurveyResultsInfo> surveyResults = null;
         if ((!(String.IsNullOrEmpty(surveyResultsXml))) && (surveyResultsXml != "<SurveyResults />"))
         {
            surveyResults = new List<SurveyResultsInfo>();
            XmlNode surveyResultsXmlNode = Globals.GetContent(surveyResultsXml, "SurveyResults");
            foreach(XmlNode surveyResultXmlNode in surveyResultsXmlNode)
            {
               SurveyResultsInfo surveyResult = SurveyResultFromXml(surveyResultXmlNode.OuterXml);
               if (surveyResult != null)
                  surveyResults.Add(surveyResult);
            }
         }
         return surveyResults;
      }
      #endregion
   }
}