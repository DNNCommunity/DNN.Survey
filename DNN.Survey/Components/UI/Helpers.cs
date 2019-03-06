using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace DNN.Modules.Survey.Components.UI
{
   static class Helpers
   {
      // Replicates the functionality of the internal Page.EnableLegacyRendering property
      public static bool EnableLegacyRendering()
      {
         bool result;
         try
         {
            string webConfigFile = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/web.config");
            XmlTextReader webConfigReader = new XmlTextReader(new StreamReader(webConfigFile));
            result = ((webConfigReader.ReadToFollowing("xhtmlConformance")) && (webConfigReader.GetAttribute("mode") == "Legacy"));
            webConfigReader.Close();
         }
         catch
         {
            result = false;
         }
         return result;
      }
   }
}
