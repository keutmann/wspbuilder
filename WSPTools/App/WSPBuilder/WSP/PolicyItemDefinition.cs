/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.Web;
using System.Net;

using Keutmann.SharePoint.WSPBuilder.Properties;
using System.Collections;
using System.IO;
using System.ComponentModel;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Collections.Specialized;
using System.Diagnostics;
using Keutmann.SharePoint.WSPBuilder.Schema;
using Mono.Cecil;
using System.Xml.Serialization;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        #region const

        private const string WSPBUILDERSTANDARD = "WSPBuilderStandard";

        #endregion

        #region Members

        private List<Assembly> _policyAssemblyList = new List<Assembly>();
        private Dictionary<string, string> _policyPermissionList = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Properties


        [XmlIgnore]
        public Dictionary<string, string> PolicyPermissionList
        {
            get { return _policyPermissionList; }
        }

        #endregion

        #region Methods

        private PermissionSet AddPermissionsToSet(PermissionSet permissionSet, SecurityDeclarationCollection securityDeclarations)
        {
            foreach (SecurityDeclaration declaration in securityDeclarations)
            {
                if (declaration.PermissionSet != null && declaration.PermissionSet.Count > 0)
                {
                    if (permissionSet == null)
                    {
                        permissionSet = declaration.PermissionSet;
                    }
                    else
                    {
                        permissionSet = permissionSet.Union(declaration.PermissionSet);
                    }
                }
            }
            return permissionSet;
        }

        /// <summary>
        /// Finds every Permission Security Attibute in a assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>Returns Permissions</returns>
        private PermissionSet GetAssemblyDefinedPermissions(AssemblyDefinition assembly)
        {
            PermissionSet permissionSet = null;

            // Find all assembly permissions
            permissionSet = AddPermissionsToSet(permissionSet, assembly.SecurityDeclarations);

            /* Load Class permissions */
            foreach (TypeDefinition type in assembly.MainModule.Types)
            {
                if (!"<Module>".Equals(type.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    // First add the Class permission attributes
                    permissionSet = AddPermissionsToSet(permissionSet, type.SecurityDeclarations);

                    // Find all constructors
                    foreach (Mono.Cecil.MethodDefinition constructor in type.Constructors)
                    {
                        // Add the methods permission attributes
                        permissionSet = AddPermissionsToSet(permissionSet, constructor.SecurityDeclarations);
                    }

                    // Find all methods
                    foreach (Mono.Cecil.MethodDefinition method in type.Methods)
                    {
                        // Add the methods permission attributes
                        permissionSet = AddPermissionsToSet(permissionSet, method.SecurityDeclarations);
                    }
                }
            }

            return permissionSet;
        }


        /// <summary>
        /// Creates a PermissionSetDefinition for the specified assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private PermissionSetDefinition CreatePermissionSetDefinition(AssemblyDefinition assembly)
        {
            PermissionSetDefinition wspPermissionSet = new PermissionSetDefinition();
            wspPermissionSet.Name = assembly.Name.Name + Guid.NewGuid().ToString();
            wspPermissionSet.@class = PermssionSetClassAttr.NamedPermissionSet;
            wspPermissionSet.version = "1";
            wspPermissionSet.Description = "WSPBuilder generated permissionSet";

            StringBuilder permissionBlob = new StringBuilder();
            StringDictionary permissions = new StringDictionary();

            PermissionSet assemblyPermissionSet = GetAssemblyDefinedPermissions(assembly);


            // Add all permissions found in assembly to the list of permissions
            if (assemblyPermissionSet != null)
            {
                foreach (IPermission permission in assemblyPermissionSet)
                {
                    SecurityElement element = permission.ToXml();
                    string assemblyFullName = element.Attributes["class"] as string;

                    // Check that the permission is not already added and that it is not in the standard permission set.
                    if (!permissions.ContainsKey(assemblyFullName))
                    {
                        permissions.Add(assemblyFullName, permission.ToString());
                    }
                }
            }

            // Add all the standard permissions 
            foreach (string key in Config.Current.PermissionSet.Keys)
            {
                string assemblyFullName = SecurityClasses.GetReference(key);
                // Only add permissions not specified in the assembly
                if (!permissions.ContainsKey(assemblyFullName))
                {
                    permissions.Add(assemblyFullName, Config.Current.PermissionSet[key]);
                }
            }

            // Write out all permissions to blob
            foreach (string permissionItem in permissions.Values)
            {
                permissionBlob.Append(permissionItem);
                if (!permissionItem.EndsWith("\r\n"))
                {
                    permissionBlob.Append("\r\n");
                }
            }

            // Save the permissions until the manifest file has been serialized.
            if (!PolicyPermissionList.ContainsKey(wspPermissionSet.Name))
            {
                PolicyPermissionList.Add(wspPermissionSet.Name, permissionBlob.ToString());
            }

            return wspPermissionSet;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private PolicyAssemblyDefinition CreatePolicyAssemblyDefinition(AssemblyDefinition assembly)
        {
            AssemblyNameDefinition assemblyName = assembly.Name;

            PolicyAssemblyDefinition policyAssemblyDefinition = new PolicyAssemblyDefinition();

            policyAssemblyDefinition.Name = assemblyName.Name;

            string hex = BitConverter.ToString(assemblyName.PublicKey);
            hex = hex.Replace("-", "");
            policyAssemblyDefinition.PublicKeyBlob = hex;

            policyAssemblyDefinition.Version = assemblyName.Version.ToString();

            return policyAssemblyDefinition;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private PolicyAssemblyDefinition[] CreatePolicyAssemblyDefinitions(AssemblyDefinition assembly)
        {
            List<PolicyAssemblyDefinition> policyAssemblyDefinitionList = new List<PolicyAssemblyDefinition>();

            policyAssemblyDefinitionList.Add(CreatePolicyAssemblyDefinition(assembly));

            return policyAssemblyDefinitionList.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private PolicyAssemblyDefinition[] CreatePolicyAssemblyDefinitions(List<AssemblyDefinition> assemblyList)
        {
            List<PolicyAssemblyDefinition> policyAssemblyDefinitionList = new List<PolicyAssemblyDefinition>();

            foreach (AssemblyDefinition assembly in assemblyList)
            {
                policyAssemblyDefinitionList.Add(CreatePolicyAssemblyDefinition(assembly));
            }

            return policyAssemblyDefinitionList.ToArray();
        }



        /// <summary>
        /// Build the Policy defintion for at assembly.
        /// </summary>
        /// <returns>PolicyItemDefinition[]</returns>
        public PolicyItemDefinition[] BuildPolicyItemDefinition()
        {
            List<PolicyItemDefinition> policyItemDefinitionList = new List<PolicyItemDefinition>();

            if (this.Solution.Assemblies != null)
            {
                foreach (AssemblyFileReference assemblyReference in this.Solution.Assemblies)
                {
                    // Find only assemblies that deploys to the WebApplication and is not Unmanage
                    if (assemblyReference.DeploymentTarget == SolutionDeploymentTargetType.WebApplication 
                        && assemblyReference.ManagedAssembly
                        && !assemblyReference.ResourceAssembly
                        && !assemblyReference.ReferenceAssembly
                        && assemblyReference.AssemblyObject != null)
                    {
                        PolicyItemDefinition policyItemDefinition = new PolicyItemDefinition();

                        policyItemDefinition.PermissionSet = CreatePermissionSetDefinition(assemblyReference.AssemblyObject);
                        policyItemDefinition.Assemblies = CreatePolicyAssemblyDefinitions(assemblyReference.AssemblyObject);

                        policyItemDefinitionList.Add(policyItemDefinition);

                        Log.Verbose("PermissionSet added: " + policyItemDefinition.PermissionSet.Name);
                    }
                }
            }
            if (policyItemDefinitionList.Count == 0)
            {
                return null;
            }
            return policyItemDefinitionList.ToArray();
        }


        /// <summary>
        /// Applies the permissions to the PolicyItemDefinitions
        /// This happens here after the serialization, because the wsp shema definition
        /// do not implement the IPermission object in a useful way.
        /// This is the last solution to the problem I wanted to implement, but the only 
        /// one I was able to get to work properly.
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        public string ApplyPolicyPermissions(string manifest)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(manifest.ToString());

            XmlNodeList permissionSetList = xmlDoc.GetElementsByTagName("PermissionSet");

            foreach (XmlNode permissionSet in permissionSetList)
            {
                string key = permissionSet.Attributes["Name"].Value;
                if (!string.IsNullOrEmpty(key))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(PolicyPermissionList[key]);

                    // Add the custom Permission Set
                    if (!String.IsNullOrEmpty(Config.Current.CustomCAS))
                    {
                        sb.Append(Config.Current.CustomCAS);
                        Log.Verbose("Custom CAS added to PermissionSet: "+key);
                    }

                    permissionSet.InnerXml = sb.ToString();
                }
            }

            StringWriter result = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(result);
            xtw.Formatting = Formatting.Indented;

            xmlDoc.WriteTo(xtw);

            return result.ToString();
        }

        #endregion
    }
}


