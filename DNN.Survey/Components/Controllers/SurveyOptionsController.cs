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
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace DNN.Modules.Survey.Components.Controllers
{
   public class SurveyOptionsController
   {
      #region Data Access
      public SurveyOptionsInfo Get(int surveyOptionID)
      {
         SurveyOptionsInfo surveyOption;
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveyOptionsInfo>();
            surveyOption = rep.GetById(surveyOptionID);
         }
         return surveyOption;
      }

      public List<SurveyOptionsInfo> GetAll(int surveyID)
      {
         IEnumerable<SurveyOptionsInfo> surveyOptions;
         using (IDataContext ctx = DataContext.Instance())
         {
            var rep = ctx.GetRepository<SurveyOptionsInfo>();
            surveyOptions = rep.Get(surveyID);
         }
         return surveyOptions.OrderBy(so => so.ViewOrder).ToList<SurveyOptionsInfo>();
      }
      #endregion
   }
}