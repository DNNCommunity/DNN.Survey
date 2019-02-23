using DNN.Modules.Survey.Components;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey
{
   public partial class SurveyResults : PortalModuleBase
   {
      private ModulePermissionCollection _modulePermissionCollection = null;
      protected ModulePermissionCollection ModulePermissionCollection
      {
         get
         {
            if (_modulePermissionCollection == null)
               _modulePermissionCollection = ModulePermissionController.GetModulePermissions(ModuleId, TabId);
            return _modulePermissionCollection;
         }
      }

      #region Control Events
      protected override void OnLoad(EventArgs e)
      {
         if (ModulePermissionController.HasModulePermission(ModulePermissionCollection, ModuleSecurity.VIEW_RESULTS_PERMISSION))
         {
            ResultsNotPublicPanel.Visible = false;
            ResultsPanel.Visible = true;
         }
         else
         {
            ResultsNotPublicPanel.Visible = true;
            ResultsPanel.Visible = false;
         }
         base.OnLoad(e);
      }
      #endregion

      protected void ViewResultsButton_Click(object sender, EventArgs e)
      {
         Response.Redirect(Globals.NavigateURL(), false);
      }
   }
}