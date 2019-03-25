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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Components.UI.WebControls.Validators
{
   public class CheckBoxValidator : BaseValidator
   {
      [Description("Whether the CheckBox must be checked or unchecked to be considered valid."), DefaultValue(true)]
      public bool MustBeChecked
      {
         get
         {
            object o = ViewState["MustBeChecked"];
            if (o == null)
               return true;
            else
               return (bool)o;
         }
         set
         {
            ViewState["MustBeChecked"] = value;
         }
      }

      private CheckBox _ctrlToValidate = null;
      protected CheckBox CheckBoxToValidate
      {
         get
         {
            if (_ctrlToValidate == null)
               _ctrlToValidate = FindControl(ControlToValidate) as CheckBox;
            return _ctrlToValidate;
         }
      }

      public string AssociatedButtonControlId
      {
         get
         {
            object o = ViewState["AssociatedButtonControlId"];
            if (o == null)
               return string.Empty;
            else
               return (string)o;
         }
         set
         {
            ViewState["AssociatedButtonControlId"] = value;
         }
      }

      private WebControl _associatedButton = null;
      protected WebControl AssociatedButton
      {
         get
         {
            if (_associatedButton == null && !string.IsNullOrEmpty(AssociatedButtonControlId))
               _associatedButton = FindControl(AssociatedButtonControlId) as WebControl;

            return _associatedButton;
         }
      }

      protected override bool ControlPropertiesValid()
      {
         // Make sure ControlToValidate is set
         if (ControlToValidate.Length == 0)
            throw new HttpException(string.Format("The ControlToValidate property of '{0}' cannot be blank.", ID));

         // Ensure that the control being validated is a CheckBox
         if (CheckBoxToValidate == null)
            throw new HttpException(string.Format("The CheckBoxValidator can only validate controls of type CheckBox."));

         // Make sure AssociatedButton, if set, referenced a Button, LinkButton, or ImageButton            
         bool btnCtrlIdSetButNoRef = (!string.IsNullOrEmpty(AssociatedButtonControlId) && AssociatedButton == null);

         bool btnCtrlIdNotRefButton = false;
         if (AssociatedButton != null)
         {
            if (AssociatedButton is Button || AssociatedButton is LinkButton || AssociatedButton is ImageButton)
               // No problem here...
               btnCtrlIdNotRefButton = false;
            else
               // AssociatedButton is an invalid type
               btnCtrlIdNotRefButton = true;
         }

         if (btnCtrlIdSetButNoRef || btnCtrlIdNotRefButton)
            throw new HttpException(string.Format("The AssociatedButtonControlId property of '{0}', if set, must reference a Button, LinkButton, or ImageButton control.", ID));


         return true;    // if we reach here, everything checks out
      }

      protected override bool EvaluateIsValid()
      {
         // Make sure that the CheckBox is set as directed by MustBeChecked
         return CheckBoxToValidate.Checked == MustBeChecked;
      }

      protected override void AddAttributesToRender(HtmlTextWriter writer)
      {
         base.AddAttributesToRender(writer);

         // Add the client-side code (if needed)
         if (RenderUplevel)
         {
            // Indicate the mustBeChecked value and the client-side function to used for evaluation
            // Use AddAttribute if Helpers.EnableLegacyRendering is true; otherwise, use expando attributes                
            if (Helpers.EnableLegacyRendering())
            {
               writer.AddAttribute("evaluationfunction", "CheckBoxValidatorEvaluateIsValid", false);
               writer.AddAttribute("mustBeChecked", MustBeChecked ? "true" : "false", false);
            }
            else
            {
               Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "CheckBoxValidatorEvaluateIsValid", false);
               Page.ClientScript.RegisterExpandoAttribute(ClientID, "mustBeChecked", MustBeChecked ? "true" : "false", false);
            }
         }
      }

      protected override void OnPreRender(EventArgs e)
      {
         base.OnPreRender(e);

         if (RenderUplevel && Page != null)
         {
            // Register the client-side function using WebResource.axd (if needed)
            // see: http://aspnet.4guysfromrolla.com/articles/080906-1.aspx
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "DNN.Modules.Survey.Components.UI.WebControls.Validators"))
               Page.ClientScript.RegisterClientScriptInclude(GetType(), "DNN.Modules.Survey.Components.UI.WebControls.Validators", Page.ClientScript.GetWebResourceUrl(GetType(), "DNN.Modules.Survey.Components.UI.WebControls.Validators.Validators.js"));

            // If there's an associated Button for this validator, add the script to enable/disable
            // it when the checkbox is clicked AND when the page is first loaded
            if (AssociatedButton != null)
            {
               string callCheckBoxValidatorDisableButton = string.Format("CheckBoxValidatorDisableButton('{0}', {1}, '{2}');", CheckBoxToValidate.ClientID, MustBeChecked ? "true" : "false", AssociatedButton.ClientID);

               CheckBoxToValidate.Attributes.Add("onclick", callCheckBoxValidatorDisableButton);

               Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), callCheckBoxValidatorDisableButton, true);
            }
         }
      }
   }
}
