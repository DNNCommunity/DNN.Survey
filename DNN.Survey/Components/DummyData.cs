using DNN.Modules.Survey.Components.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DNN.Modules.Survey.Components
{
   public static class DummyData
   {
      public static List<SurveyOptionsInfo> LoadDummyForTextAnswer(int surveyID, int surveyOptionID, int userID)
      {
         List<SurveyOptionsInfo> answers = new List<SurveyOptionsInfo>();
         answers.Add(
            new SurveyOptionsInfo() {
               SurveyID = surveyID,
               SurveyOptionID = surveyOptionID,
               ViewOrder = 1,
               OptionName = "[DUMMY:Text Answer]",
               IsCorrect = false,
               CreatedByUserID = userID,
               CreatedDate = DateTime.Now
            }
         );
         return answers;
      }
   }
}