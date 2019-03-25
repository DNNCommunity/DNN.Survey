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
   public class CheckBoxListValidator : BaseValidator
   {
      [Description("The minimum number of CheckBoxes that must be checked to be considered valid.")]
      public int MinimumNumberOfSelectedCheckBoxes
      {
         get
         {
            object o = ViewState["MinimumNumberOfSelectedCheckBoxes"];
            if (o == null)
               return 1;
            else
               return (int)o;
         }
         set
         {
            ViewState["MinimumNumberOfSelectedCheckBoxes"] = value;
         }
      }

      private CheckBoxList _ctrlToValidate = null;
      protected CheckBoxList CheckBoxListToValidate
      {
         get
         {
            if (_ctrlToValidate == null)
               _ctrlToValidate = FindControl(base.ControlToValidate) as CheckBoxList;
            return _ctrlToValidate;
         }
      }

      protected override bool ControlPropertiesValid()
      {
         // Make sure ControlToValidate is set
         if (base.ControlToValidate.Length == 0)
            throw new HttpException(string.Format("The ControlToValidate property of '{0}' cannot be blank.", ID));

         // Ensure that the control being validated is a CheckBoxList
         if (CheckBoxListToValidate == null)
            throw new HttpException(string.Format("The CheckBoxListValidator can only validate controls of type CheckBoxList."));

         // ... and that it has at least MinimumNumberOfSelectedCheckBoxes ListItems
         if (CheckBoxListToValidate.Items.Count < MinimumNumberOfSelectedCheckBoxes)
            throw new HttpException(string.Format("MinimumNumberOfSelectedCheckBoxes must be set to a value greater than or equal to the number of ListItems; MinimumNumberOfSelectedCheckBoxes is set to {0}, but there are only {1} ListItems in '{2}'", MinimumNumberOfSelectedCheckBoxes, CheckBoxListToValidate.Items.Count, CheckBoxListToValidate.ID));

         return true;    // if we reach here, everything checks out
      }

      protected override bool EvaluateIsValid()
      {
         // Make sure that the CheckBoxList has at least MinimumNumberOfSelectedCheckBoxes ListItems selected
         int selectedItemCount = 0;
         foreach (ListItem cb in CheckBoxListToValidate.Items)
            if (cb.Selected) selectedItemCount++;

         return selectedItemCount >= MinimumNumberOfSelectedCheckBoxes;
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
               writer.AddAttribute("evaluationfunction", "CheckBoxListValidatorEvaluateIsValid", false);
               writer.AddAttribute("minimumNumberOfSelectedCheckBoxes", MinimumNumberOfSelectedCheckBoxes.ToString(), false);
            }
            else
            {
               Page.ClientScript.RegisterExpandoAttribute(ClientID, "evaluationfunction", "CheckBoxListValidatorEvaluateIsValid", false);
               Page.ClientScript.RegisterExpandoAttribute(ClientID, "minimumNumberOfSelectedCheckBoxes", MinimumNumberOfSelectedCheckBoxes.ToString(), false);
            }
         }
      }

      protected override void OnPreRender(EventArgs e)
      {
         base.OnPreRender(e);

         // Register the client-side function using WebResource.axd (if needed)
         // see: http://aspnet.4guysfromrolla.com/articles/080906-1.aspx
         if ((RenderUplevel) && (Page != null) && (!(Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "DNN.Modules.Survey.Components.UI.WebControls.Validators"))))
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "DNN.Modules.Survey.Components.UI.WebControls.Validators", Page.ClientScript.GetWebResourceUrl(GetType(), "DNN.Modules.Survey.Components.UI.WebControls.Validators.Validators.js"));
      }

   }
}
