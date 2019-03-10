using DNN.Modules.Survey.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNN.Modules.Survey.Controls
{
   public partial class CanvasControl : UserControl
   {
      public string Labels { get; set; }
      public string Data { get; set; }
      public string BackgroundColors { get; set; }
      public string BorderColors { get; set; }
      public ChartType ChartType { get; set; }

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

      protected void Page_Load(object sender, EventArgs e)
      {
         string[] labels = Labels.Split(new char[] { ',' });
         string[] data = Data.Split(new char[] { ',' });
         int sum = 0;
         foreach (string s in data)
         {
            sum += Convert.ToInt32(s);
         }
         for (int i = 0; i < labels.Length; i++)
         {
            // Let Google see the results...
            Graph.InnerHtml += string.Format("<span>{0}: {1} ({2:0.00}%)</span>", labels[i].Substring(1, labels[i].Length - 2), data[i], Convert.ToDouble(data[i]) * 100 / sum);
         }

      }

      protected void Page_PreRender(object sender, EventArgs e)
      {
         StringBuilder scriptBuilder = new StringBuilder();
         scriptBuilder.Append(string.Format("var ctx = document.getElementById(\"{0}\").getContext(\"2d\");\r\n", Graph.ClientID));
         scriptBuilder.Append("var myChart = new Chart(ctx, {\r\n");
         scriptBuilder.Append(string.Format("   type: \"{0}\",\r\n", Base.ToChartJSChartType(ChartType)));
         scriptBuilder.Append("   data: {\r\n");
         scriptBuilder.Append(string.Format("      labels: [{0}],\r\n", Labels));
         scriptBuilder.Append("      datasets: [{\r\n");
         scriptBuilder.Append("         label: \"# of Votes\",\r\n");
         scriptBuilder.Append(string.Format("         data: [{0}],\r\n", Data));
         scriptBuilder.Append(string.Format("         backgroundColor: [{0}],\r\n", BackgroundColors));
         scriptBuilder.Append(string.Format("         borderColor: [{0}],\r\n", BorderColors));
         scriptBuilder.Append("         borderWidth: 1\r\n");
         scriptBuilder.Append("      }]\r\n");
         scriptBuilder.Append("   },\r\n");
         scriptBuilder.Append("   options: {\r\n");
         scriptBuilder.Append("     scales: {\r\n");
         switch (ChartType)
         {
            case ChartType.Bar:
               scriptBuilder.Append("         yAxes: [{\r\n");
               scriptBuilder.Append("            ticks: {\r\n");
               scriptBuilder.Append("               beginAtZero: true\r\n");
               scriptBuilder.Append("            }\r\n");
               scriptBuilder.Append("         }],\r\n");
               scriptBuilder.Append("        xAxes: [{\r\n");
               scriptBuilder.Append("           ticks: {\r\n");
               scriptBuilder.Append("              autoSkip: false\r\n");
               scriptBuilder.Append("           }\r\n");
               scriptBuilder.Append("        }],\r\n");
               break;
            case ChartType.HorizontalBar:
               scriptBuilder.Append("        xAxes: [{\r\n");
               scriptBuilder.Append("           ticks: {\r\n");
               scriptBuilder.Append("              beginAtZero: true\r\n");
               scriptBuilder.Append("           }\r\n");
               scriptBuilder.Append("        }],\r\n");
               break;
            default:
               break;
         }
         scriptBuilder.Append("         plugins: {\r\n");
         scriptBuilder.Append("            datalabels: {\r\n");
         scriptBuilder.Append("               display: \"true\"\r\n");
         scriptBuilder.Append("            }\r\n");
         scriptBuilder.Append("         }\r\n");
         scriptBuilder.Append("      }\r\n");
         scriptBuilder.Append("   }\r\n");
         scriptBuilder.Append("});\r\n");

         if (!(Page.ClientScript.IsStartupScriptRegistered(string.Format("Graph-{0}", ClientID))))
            Page.ClientScript.RegisterStartupScript(GetType(), string.Format("Graph-{0}", ClientID), scriptBuilder.ToString(), true);
      }
   }
}