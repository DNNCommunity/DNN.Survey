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

using DNN.Modules.Survey.Components.Entities;
using DNN.Modules.Survey.Components.Providers;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DNN.Modules.Survey.Components.Controllers
{
   public class SurveyResultsController
   {
      #region Data Access
      public List<SurveyResultsInfo> GetAll(int moduleID)
      {
         IEnumerable<SurveyResultsInfo> surveyResults;
         using (IDataContext ctx = DataContext.Instance())
         {
            surveyResults = ctx.ExecuteQuery<SurveyResultsInfo>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResults_GetAll",
               moduleID);
         }
         return surveyResults.ToList<SurveyResultsInfo>();
      }

      public void Add(List<SurveyResultsInfo> surveyResults, bool authorizedUsersOnly)
      {
         using (IDataContext ctx = DataContext.Instance())
         {
            ctx.Execute(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResults_Add",
               authorizedUsersOnly,
               XmlDataProvider.SurveyResultsToXml(surveyResults)
            );
         }
         DataCache.ClearCache("Surveys");
         DataCache.ClearCache("SurveyOptions");
         DataCache.ClearCache("SurveyResults");
      }

      public void DropAll(int moduleID)
      {
         using (IDataContext ctx = DataContext.Instance())
         {
            ctx.Execute(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResultData_Delete",
               moduleID);
         }
         DataCache.ClearCache("Surveys");
         DataCache.ClearCache("SurveyOptions");
         DataCache.ClearCache("SurveyResults");
      }

      public List<SurveyResultInfo> Get(int surveyID)
      {
         IEnumerable<SurveyResultInfo> surveyResult;
         using (IDataContext ctx = DataContext.Instance())
         {
            surveyResult = ctx.ExecuteQuery<SurveyResultInfo>(CommandType.StoredProcedure,
               "{databaseOwner}{objectQualifier}SurveyResults_Get",
               surveyID);
         }
         return surveyResult.ToList<SurveyResultInfo>();
      }
      #endregion
   }
}