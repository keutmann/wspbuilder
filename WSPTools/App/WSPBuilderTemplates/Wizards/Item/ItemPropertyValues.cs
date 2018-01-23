/* Program : WSPBuilderTemplates
 * Created by: Tom Clarkson
 * Date : 2008
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * Modified by Carsten Keutmann
 * Date : 2009
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace WSPBuilderTemplates
{
    /// <summary>
    /// Valid key values for properties added by an item wizard
    /// </summary>
    public class ItemPropertyValues
    {
        /// <summary>
        /// GUID for the feature being added
        /// </summary>
        public static string FeatureID = "$featureid$";

        /// <summary>
        /// GUID for the solution
        /// </summary>
        public static string SolutionID = "$solutionid$";
        

        /// <summary>
        /// Name of the project containing the item being added (Project.Name)
        /// </summary>
        public static string ProjectName = "$projectname$";
        /// <summary>
        /// Root namespace for the project (Project.Properties.Item("RootNamespace"))
        /// </summary>
        public static string RootNamespace = "$RootNamespace$";
        /// <summary>
        /// Default namespace for the project (Project.Properties.Item("DefaultNamespace"))
        /// </summary>
        public static string DefaultNamespace = "$DefaultNamespace$";
        /// <summary>
        /// The namespace for the current class 
        /// </summary>
        public static string SubNamespace = "$subnamespace$";
        /// <summary>
        /// The namespace for the current class 
        /// </summary>
        public static string ClassNamespace = "$ClassNamespace$";
        /// <summary>
        /// Output file for the project (Project.Properties.Item("OutputFileName"))
        /// </summary>
        public static string OutputFilename = "$OutputFileName$";
        /// <summary>
        /// Assembly vesrsion from the project (Project.Properties.Item("AssemblyVersion"))
        /// </summary>
        public static string AssemblyVersion = "$AssemblyVersion$";
        /// <summary>
        /// Full path to the .csproj file (Project.Properties.Item("FullPath"))
        /// </summary>
        public static string FullPath = "$FullPath$";
        /// <summary>
        /// Assembly name from the project (Project.Properties.Item("AssemblyName"))
        /// </summary>
        public static string AssemblyName = "$AssemblyName$";
        /// <summary>
        /// Class name for the current item 
        /// </summary>
        public static string ClassName = "$ClassName$";

        /// <summary>
        /// Class name for the current item 
        /// </summary>
        public static string SafeItemRootName = "$safeitemrootname$";

        /// <summary>
        /// Public key token of the assembly
        /// </summary>
        public static string PublicKeyToken = "$PublicKeyToken$";

        /// <summary>
        /// Name of the item as entered in the add item dialog
        /// </summary>
        public static string RootName = "$rootname$";
        
        /// <summary>
        /// XML snippet used by the CustomFieldTypeWizard
        /// </summary>
        public static string FieldEditorUserControl = "$FieldEditorUserControl$";
        /// <summary>
        /// Set to TRUE by the CustomFieldTypeWizard to work around a bug in SharePoint that requires the use
        /// of a custom field editor control.
        /// </summary>
        public static string HideCustomProperty = "$HideCustomProperty$";

        /// <summary>
        /// The folder name of the feature
        /// </summary>
        public static string FeatureName = "$featurename$";
        /// <summary>
        /// Title of the feature as entered in the feature wizard dialog
        /// </summary>
        public static string FeatureTitle = "$featuretitle$";
        /// <summary>
        /// Description of the feature as entered in the feature wizard dialog
        /// </summary>
        public static string FeatureDescription = "$featuredesc$";
        /// <summary>
        /// Scope of the feature as entered in the feature wizard dialog
        /// </summary>
        public static string FeatureScope = "$featurescope$";

        /// <summary>
        /// The name of the SharePointRoot, this can be 12,14, SharePointRoot
        /// </summary>
        public static string SharePointRoot = "$SharePointRoot$";



        /// <summary>
        /// </summary>
        public static string AssemblyFullName = "$AssemblyFullName$";

        /// <summary>
        /// The string that will add an attribute to an Event handler assembly.
        /// Must used with the ClassHandler 
        /// </summary>
        public static string AssemblyHandlerAttribute = "$AssemblyHandlerAttribute$";

        /// <summary>
        /// The string that will add an attribute to an Event handler class.
        /// Must used with the AssemblyHandler 
        /// </summary>
        public static string ClassHandlerAttribute = "$ClassHandlerAttribute$";

        /// <summary>
        /// </summary>
        public static string TargetPath = "$TargetPath$";

        /// <summary>
        /// A placeholder for content specific for SharePoint 2010
        /// </summary>
        public static string SharePoint2010Content = "$SharePoint2010Content$";


        public static string FeatureActivatedContent = "$FeatureActivatedContent$";
        public static string FeatureDeactivatingContent = "$FeatureDeactivatingContent$";
        public static string FeatureUpgradingMethod = "$FeatureUpgradingMethod$";
        public static string CodeContent = "$CodeContent$";
        public static string WebPartTitle = "$WebPartTitle$";
        public static string WebPartDescription = "$WebPartDescription$";
        public static string DelegateControlID = "$DelegateControlID$";
        
    }
}
