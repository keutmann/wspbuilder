/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Web.UI;

using Keutmann.SharePoint.WSPBuilder.Library;
using Mono.Cecil;
//using Microsoft.SharePoint.WebPartPages;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        #region const

        private const string BUILDSAFECONTROLS = "BuildSafeControls";
        private const string EXPANDTYPES = "ExpandTypes";
        private const string EMPTYNAMESPACE = "[EMPTYNAMESPACE]";

        #endregion

        #region Properties

        [DisplayName(@"-BuildSafeControls [true|false] (Default value is true)")]
        [Description("Build Safe Controls definitions for every assembly included in the WSP.")]
        public bool BuildSafeControls
        {
            get
            {
                return Config.Current.GetBool(BUILDSAFECONTROLS, true);
            }
        }


        [DisplayName("-ExpandTypes [true|false] (Default is false)")]
        [Description("Writes a SafeControl tag for every public type in the assembly.")]
        public bool ExpandTypes
        {
            get
            {
                return Config.Current.GetBool(EXPANDTYPES, false);
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Builds the safe controls objects. 
        /// </summary>
        /// <returns>The safe control list.</returns>
        public SafeControlDefinition[] BuildSafeControlDefinition(AssemblyDefinition assembly, SafeControlDefinition[] existingSafeControls)
        {
            List<SafeControlDefinition> safeControlList = null;

            // Ensure that the existingSafeControls are not null
            existingSafeControls = existingSafeControls != null ? existingSafeControls : new SafeControlDefinition[] { };

            // If a parameter is set to specify that the types should be written, then
            if (ExpandTypes)
            {
                safeControlList = BuildSafeControlsWithTypes(assembly);
            }
            else
            {
                safeControlList = BuildSafeControlsWithNamespaces(assembly);
            }

            if (safeControlList.Count == 0)
            {
                return null;
            }

            foreach (SafeControlDefinition existingItem in existingSafeControls)
            {
                if (!safeControlList.Exists(p => SafeControlDefinition.IsMatch(p, existingItem)))
                {
                    safeControlList.Add(existingItem);
                }
            }

            Log.Verbose("SafeControls added: " + safeControlList.Count);

            return safeControlList.ToArray();
        }


        /// <summary>
        /// Create the SafeControls with namespaces
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private List<SafeControlDefinition> BuildSafeControlsWithNamespaces(AssemblyDefinition assembly)
        {
            // Get the System.Web.UI.Control type.
            Type controlType = typeof(Control);

            List<SafeControlDefinition> safeControlList = new List<SafeControlDefinition>();

            string assemblyName = assembly.Name.FullName;

            Hashtable coll = new Hashtable();
            foreach (TypeDefinition classType in assembly.MainModule.Types)
            {
                if (!"<Module>".Equals(classType.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (IncludeType(assembly, classType, controlType))
                    {
                        string key = (String.IsNullOrEmpty(classType.Namespace)) ? EMPTYNAMESPACE : classType.Namespace;
                        coll[key] = true;
                    }
                }
            }
            foreach (string nameSpace in coll.Keys)
            {
                // otherwise just write the asteriks
                SafeControlDefinition safeControl = new SafeControlDefinition();
                safeControl.SafeSpecified = true;
                safeControl.Assembly = assemblyName;
                safeControl.Namespace = (nameSpace.Equals(EMPTYNAMESPACE)) ? "" : nameSpace;
                safeControl.Safe = TrueFalseMixed.True;
                safeControl.TypeName = "*";

                safeControlList.Add(safeControl);
            }

            return safeControlList;
        }

        private List<SafeControlDefinition> BuildSafeControlsWithTypes(AssemblyDefinition assembly)
        {
            // Get the System.Web.UI.Control type.
            Type controlType = typeof(Control);

            List<SafeControlDefinition> safeControlList = new List<SafeControlDefinition>();

            string assemblyName = assembly.Name.FullName;

            foreach (TypeDefinition classType in assembly.MainModule.Types)
            {
                if (!"<Module>".Equals(classType.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (IncludeType(assembly, classType, controlType))
                    {
                        SafeControlDefinition safeControl = new SafeControlDefinition();
                        safeControl.SafeSpecified = true;
                        safeControl.Assembly = assemblyName;
                        safeControl.Namespace = classType.Namespace;
                        safeControl.Safe = TrueFalseMixed.True;
                        safeControl.TypeName = classType.Name;

                        safeControlList.Add(safeControl);
                    }
                }
            }

            return safeControlList;
        }

        private bool IncludeType(AssemblyDefinition assembly, TypeDefinition classType, Type targetType)
        {
            bool result = false;

            // Check to see if the type is a public class
            // Check to see if the type do not inherited from an object
            // Check that the class inherited from the System.Web.UI.Control class
            if (classType.IsClass
                && classType.IsPublic
                && !classType.BaseType.Name.Equals("object", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                // Include the class even that an exception is thrown
                result = true;
                try
                {
                    result = AssemblyStore.IsType((CustomAssemblyResolver)assembly.Resolver, assembly.Name.FullName, classType.FullName, targetType);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex.Message + ". Please define the -DLLReferencePath with the path to the referenced assemblies. " +
                        "However a SafeControl tag for the " + classType.FullName + " class has been created.");
                }
            }
            return result;
        }

        private bool IsType(AssemblyDefinition assembly, TypeDefinition classType, Type targetType)
        {
            if (targetType.FullName.Equals(classType.FullName) &&
                targetType.Assembly.FullName.Equals(classType.Module.Assembly.Name.FullName))
            {
                return true;
            }
            if (classType.BaseType != null)
            {
                if (classType.BaseType.Scope is AssemblyNameReference)
                {
                    AssemblyNameReference subAssemblyReference = (AssemblyNameReference)classType.BaseType.Scope;
                    AssemblyDefinition subAssembly = assembly.Resolver.Resolve(subAssemblyReference.FullName);
                    foreach (TypeDefinition subType in subAssembly.MainModule.Types)
                    {
                        if (subType.FullName != null && subType.FullName.EndsWith(classType.BaseType.FullName))
                        {
                            return IsType(subAssembly, subType, targetType);
                        }
                    }
                }
                else
                {
                    if (classType.BaseType.Scope is Mono.Cecil.ModuleDefinition)
                    {
                        Mono.Cecil.ModuleDefinition subModule = (Mono.Cecil.ModuleDefinition)classType.BaseType.Scope;
                        foreach (TypeDefinition subType in subModule.Types)
                        {
                            if (subType.FullName != null && subType.FullName.EndsWith(classType.BaseType.FullName))
                            {
                                return IsType(subModule.Assembly, subType, targetType);
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
