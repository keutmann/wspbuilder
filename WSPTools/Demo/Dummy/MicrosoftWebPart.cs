/* Program : TestWSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * This test requires NUnit 
 */
using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Keutmann.Demo.Dummy
{
    [Guid("47fa670e-78e5-4503-b374-1e1c3d0d4934")]
    public class MicrosoftWebPart : System.Web.UI.WebControls.WebParts.WebPart
    {
        public MicrosoftWebPart()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // This is dummy webpart
            // For testing
        }
    }
}
