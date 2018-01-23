/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2008
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
#region namespace references
using System;
using System.Collections.Specialized;
#endregion

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class SecurityClasses
    {
        private StringDictionary AssemblyReferences = new StringDictionary();

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly SecurityClasses instance = new SecurityClasses();
        }

        public static SecurityClasses Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        SecurityClasses()
        {
            AssemblyReferences.Add("AllMembershipCondition", "System.Security.Policy.AllMembershipCondition, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("AspNetHostingPermission", "System.Web.AspNetHostingPermission, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("DnsPermission", "System.Net.DnsPermission, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("EnvironmentPermission", "System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("FileIOPermission", "System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("FirstMatchCodeGroup", "System.Security.Policy.FirstMatchCodeGroup, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("IsolatedStorageFilePermission", "System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("NamedPermissionSet", "System.Security.NamedPermissionSet");
            AssemblyReferences.Add("PrintingPermission", "System.Drawing.Printing.PrintingPermission, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            AssemblyReferences.Add("SecurityPermission", "System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("SharePointPermission", "Microsoft.SharePoint.Security.SharePointPermission, Microsoft.SharePoint.Security, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            AssemblyReferences.Add("SmtpPermission", "System.Net.Mail.SmtpPermission, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("SqlClientPermission", "System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("StrongNameMembershipCondition", "System.Security.Policy.StrongNameMembershipCondition, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("UIPermission", "System.Security.Permissions.UIPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("UnionCodeGroup", "System.Security.Policy.UnionCodeGroup, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("UrlMembershipCondition", "System.Security.Policy.UrlMembershipCondition, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("WebPermission", "System.Net.WebPermission, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            AssemblyReferences.Add("WebPartPermission", "Microsoft.SharePoint.Security.WebPartPermission, Microsoft.SharePoint.Security, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c");
            AssemblyReferences.Add("ZoneMembershipCondition", "System.Security.Policy.ZoneMembershipCondition, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
        }

        public static string GetReference(string name)
        {
            string result = null;

            // Look for the key when without namespace
            if (Instance.AssemblyReferences.ContainsKey(name))
            {
                result = Instance.AssemblyReferences[name];
            }
            else
            {
                // Look for the reference with namespace
                foreach (string assemblyRef in Instance.AssemblyReferences.Values)
                {
                    if (assemblyRef.StartsWith(name))
                    {
                        result = assemblyRef;
                        break;
                    }
                }
            }

            return result;
        }
    }
}