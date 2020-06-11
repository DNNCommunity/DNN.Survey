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

using DNN.Modules.Survey.Components;
using System;
using System.Text;
using System.Web;
using System.Web.UI;

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
         // string[] labels = Labels.Split(new char[] { ',' });
         string[] labels = Labels.Substring(1, Labels.Length - 2).Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
         string[] data = Data.Split(new char[] { ',' });
         int sum = 0;
         foreach (string s in data)
         {
            sum += Convert.ToInt32(s);
         }
         for (int i = 0; i < labels.Length; i++)
         {
            // Let Google see the results...
            Graph.InnerHtml += string.Format("<span>{0}: {1} ({2:0.00}%)</span>", labels[i], data[i], (data[i] == "0" ? 0 : Convert.ToDouble(data[i]) * 100 / sum));
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