/* Program : TestWSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 */
using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Keutmann.Demo.Dummy.Namespace1
{
    [Guid("f9710f62-55a6-4748-b6c0-9b408fc98146")]
    public class WebPart1 : System.Web.UI.WebControls.WebParts.WebPart
    {
        public WebPart1()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // TODO: add custom rendering code here.
            // writer.Write("Output HTML");
        }
    }
}
