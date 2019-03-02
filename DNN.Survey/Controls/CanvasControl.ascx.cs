using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class CanvasControl : UserControl
   {
      public override string ClientID
      {
         get
         {
            return Graph.ClientID;
         }
      }

      public string Header
      {
         get
         {
            return HeaderLabel.Text;
         }
         set
         {
            HeaderLabel.Text = value;
         }
      }
   }
}