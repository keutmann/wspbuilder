using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace WSPDemo
{
    [Guid("1d295d14-5117-4091-9e96-8c12f0fbea8d")]
    public class WSPDemo : WebPart
    {
        public WSPDemo()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        protected override void Render(HtmlTextWriter writer)
        {

            writer.Write("<h1>Hello world!</h1>");
        }
    }
}

