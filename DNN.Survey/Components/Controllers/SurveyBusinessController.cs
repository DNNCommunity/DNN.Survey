using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Components.Providers;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Search.Entities;
using DotNetNuke.Services.Search.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace DNN.Modules.Survey.Components.Controllers
{
   public class SurveyBusinessController : ModuleSearchBase, IUpgradeable, IPortable
   {
      #region Private Properties
      private SurveysController _surveysController = null;
      private SurveyOptionsController _surveyOptionsController = null;
      private SurveysExportController _surveysExportController = null;

      protected SurveysController SurveysController
      {
         get
         {
            if (_surveysController == null)
               _surveysController = new SurveysController();
            return _surveysController;
         }
      }

      protected SurveyOptionsController SurveyOptionsController
      {
         get
         {
            if (_surveyOptionsController == null)
               _surveyOptionsController = new SurveyOptionsController();
            return _surveyOptionsController;
         }
      }

      protected SurveysExportController SurveysExportController
      {
         get
         {
            if (_surveysExportController == null)
               _surveysExportController = new SurveysExportController();
            return _surveysExportController;
         }
      }
      #endregion

      #region ModuleSearchBase
      private static readonly int ModuleSearchTypeId = SearchHelper.Instance.GetSearchTypeByName("module").SearchTypeId;

      public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDateUtc)
      {
         List<SearchDocument> docs = new List<SearchDocument>();
         List<SurveysInfo> surveys = SurveysController.GetAll(moduleInfo.ModuleID);

         foreach (SurveysInfo survey in surveys)
         {
            SearchDocument surveyDoc = new SearchDocument();
            surveyDoc.UniqueKey = string.Format("{0}_{1}_{2}", moduleInfo.ModuleDefinition.DefinitionName, moduleInfo.PortalID, survey.SurveyID);
            surveyDoc.NumericKeys.Add("SurveyID", survey.SurveyID);
            surveyDoc.Title = moduleInfo.ModuleTitle;
            surveyDoc.Body = survey.Question;
            surveyDoc.AuthorUserId = (survey.LastModifiedByUserID == null ? survey.CreatedByUserID : survey.LastModifiedByUserID.Value);
            surveyDoc.ModuleId = moduleInfo.ModuleID;
            surveyDoc.ModuleDefId = moduleInfo.ModuleDefID;
            surveyDoc.PortalId = moduleInfo.PortalID;
            surveyDoc.TabId = moduleInfo.ParentTab.TabID;
            surveyDoc.SearchTypeId = ModuleSearchTypeId;
            surveyDoc.ModifiedTimeUtc = (survey.LastModifiedDate == null ? survey.CreatedDate : survey.LastModifiedDate.Value).ToUniversalTime();
            // Important, if false, the document will be deleted from the search index
            surveyDoc.IsActive = true;
            docs.Add(surveyDoc);
            List<SurveyOptionsInfo> surveyOptions = SurveyOptionsController.GetAll(survey.SurveyID);
            foreach (SurveyOptionsInfo surveyOption in surveyOptions)
            {
               SearchDocument surveyOptionDoc = new SearchDocument();
               surveyOptionDoc.UniqueKey = string.Format("{0}_{1}_{2}_{3}", moduleInfo.ModuleDefinition.DefinitionName, moduleInfo.PortalID, survey.SurveyID, surveyOption.SurveyOptionID);
               surveyOptionDoc.NumericKeys.Add("SurveyOptionID", surveyOption.SurveyOptionID);
               surveyOptionDoc.Title = survey.Question;
               surveyOptionDoc.Body = surveyOption.OptionName;
               surveyOptionDoc.AuthorUserId = (surveyOption.LastModifiedByUserID == null ? surveyOption.CreatedByUserID : surveyOption.LastModifiedByUserID.Value);
               surveyOptionDoc.ModuleId = moduleInfo.ModuleID;
               surveyOptionDoc.ModuleDefId = moduleInfo.ModuleDefID;
               surveyOptionDoc.PortalId = moduleInfo.PortalID;
               surveyOptionDoc.TabId = moduleInfo.ParentTab.TabID;
               surveyOptionDoc.SearchTypeId = ModuleSearchTypeId;
               surveyOptionDoc.ModifiedTimeUtc = (surveyOption.LastModifiedDate == null ? surveyOption.CreatedDate : surveyOption.LastModifiedDate.Value).ToUniversalTime();
               surveyOptionDoc.IsActive = true;
               docs.Add(surveyOptionDoc);
            }
         }
         return docs;
      }
      #endregion

      #region IUpgradeable
      public string UpgradeModule(string version)
      {
         return string.Format("Upgrading to version {0}.", version);
      }
      #endregion

      #region IPortable
      public string ExportModule(int moduleID)
      {
         StringBuilder exportXml = new StringBuilder();
         Hashtable moduleSettings = ModuleController.Instance.GetModule(moduleID, Null.NullInteger, true).ModuleSettings;

         exportXml.Append("<Survey>");
         if (moduleSettings == null)
         {
            exportXml.Append("<ModuleSettings />");
         }
         else
         {
            exportXml.Append("<ModuleSettings>");
            foreach (KeyValuePair<string, string> setting in moduleSettings)
            {
               exportXml.Append(String.Format("<ModuleSetting><{0}>{1}</{0}></ModuleSetting>", setting.Key, setting.Value));
            }
            exportXml.Append("</ModuleSettings>");
         }

         List<SurveysInfo> surveys = SurveysController.GetAll(moduleID);
         exportXml.Append(XmlDataProvider.SurveysToXml(surveys, true));

         return exportXml.ToString();
      }

      public void ImportModule(int moduleID, string content, string version, int userID)
      {
         string[] versions = version.Split(new char[] { '.' });

         if (Convert.ToInt32(versions[0]) < 9)
         {
            // Old Xml data sructure by the original core module
            XmlNode surveysNode = Globals.GetContent(content, "surveys");
            foreach (XmlNode surveyNode in surveysNode)
            {
               SurveysInfo survey = new SurveysInfo();
               survey.SurveyID = 0;
               survey.ModuleID = moduleID;
               survey.Question = surveyNode.SelectSingleNode("question").InnerText;
               survey.ViewOrder = Convert.ToInt32(surveyNode.SelectSingleNode("vieworder").InnerText);
               survey.OptionType = (QuestionType)Convert.ToInt32(surveyNode.SelectSingleNode("optiontype").InnerText);

               XmlNode surveyOptionsNode = surveyNode.SelectSingleNode("surveyoptions");
               List<SurveyOptionsInfo> surveyOptions = new List<SurveyOptionsInfo>();
               foreach (XmlNode surveyOptionNode in surveyOptionsNode)
               {
                  SurveyOptionsInfo surveyOption = new SurveyOptionsInfo();
                  surveyOption.SurveyOptionID = 0;
                  surveyOption.OptionName = surveyOptionNode.SelectSingleNode("optionname").InnerText;
                  surveyOption.IsCorrect = Convert.ToBoolean(surveyOptionNode.SelectSingleNode("iscorrect").InnerText);
                  surveyOption.ViewOrder = Convert.ToInt32(surveyOptionNode.SelectSingleNode("vieworder").InnerText);
                  surveyOptions.Add(surveyOption);
               }
               SurveysController.AddOrChange(survey, XmlDataProvider.SurveyOptionsToXml(surveyOptions), userID);
            }
         }
         else
         {
            List<SurveysInfo> surveys = XmlDataProvider.SurveysFromXml(content);
            foreach (SurveysInfo survey in surveys)
            {
               survey.SurveyID = 0;
               survey.ModuleID = moduleID;
               SurveysController.AddOrChange(survey, survey.SurveyOptionsXml, userID);
            }
         }
      }

      public string CSVExport(int moduleID, string resourceFile)
      {
         StringBuilder csvBuilder = new StringBuilder();
         csvBuilder.Append("SurveyID; Question; Question Type; Statistical; Answer; Votes; Correct Answer; UserID; IP Address; GUID; Date\r\n");

         List<SurveysExportInfo> surveys = SurveysExportController.GetAll(moduleID);
         foreach (SurveysExportInfo survey in surveys)
         {
            csvBuilder.Append(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10:yyyy-MM-dd hh:mm:ss}\r\n",
               survey.SurveyID,
               survey.Question,
               Localization.GetString(string.Format("QuestionType.{0}.Text", Enum.GetName(typeof(QuestionType), survey.OptionType), resourceFile)),
               survey.IsStatistical,
               (survey.OptionType == QuestionType.Text ? survey.TextAnswer : survey.OptionName),
               survey.Votes,
               survey.IsCorrect,
               survey.UserID,
               survey.IPAddress,
               survey.ResultUserID,
               survey.CreatedDate));
         }
         return csvBuilder.ToString();
      }
      #endregion
   }
}