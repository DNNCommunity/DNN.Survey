using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class PrivacyConfirmationCheckBox : System.Web.UI.UserControl
   {
      public string Label
      {
         get
         {
            return PrivacyConfirmation.Text;
         }
         set
         {
            PrivacyConfirmation.Text = value;
         }
      }

      public string ErrorMessage
      {
         get
         {
            return PrivacyConfirmationValidator.ErrorMessage;
         }
         set
         {
            PrivacyConfirmationValidator.ErrorMessage = value;
         }
      }

      public string ValidationGroup
      {
         get
         {
            return PrivacyConfirmationValidator.ValidationGroup;
         }
         set
         {
            PrivacyConfirmationValidator.ValidationGroup = value;
         }
      }

      protected void Page_Load(object sender, EventArgs e)
      {

      }
   }
}