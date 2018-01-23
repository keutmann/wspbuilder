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
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;

namespace Keutmann.Demo.Dummy
{
    [Guid("67bded7a-ea10-484f-affd-10b2afaeb541")]
    public class InternalWebPart : System.Web.UI.WebControls.WebParts.WebPart
    {
        public InternalWebPart()
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
