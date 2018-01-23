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

namespace Keutmann.Demo.Dummy.Namespace2
{
    [Guid("686f2d4c-aeb7-4fa9-98b1-cec9b484fb09")]
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
