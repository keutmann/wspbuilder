using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using System.Security.Permissions;
using Microsoft.SharePoint.Security;
using System.Net;
using System.Security.Policy;

namespace WebPart_Special_PermissionSet
{
    [Guid("91B1AF53-9811-4db3-9D27-A2ADC84634A2")]
    public class HelloWorld_Special_PermissionSet : WebPart
    {
        [FileIOPermission(SecurityAction.Assert, AllFiles=FileIOPermissionAccess.Append)]
        [SiteIdentityPermission(SecurityAction.Demand)]
        [EnvironmentPermission(SecurityAction.Demand, Read = "c:\\text.txt2")]
        [Microsoft.SharePoint.Security.SharePointPermission(SecurityAction.Demand, UnsafeSaveOnGet=true)]
        public HelloWorld_Special_PermissionSet()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        [Microsoft.SharePoint.Security.SharePointPermission(SecurityAction.Demand, ObjectModel=true)]
        [FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.Write)]
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        [AspNetHostingPermission(SecurityAction.Assert, Level = AspNetHostingPermissionLevel.High)]
        [EnvironmentPermission(SecurityAction.Demand, Read = "c:\\text.txt")]
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(String.Format("<h1>Hello {0}</h1>", Environment.MachineName));
            writer.Write("<h3>If you can see this, then the deployment of the DLL to the webapplication and the definition of the special permissions was a sucess!</h3>");
        }
    }
}