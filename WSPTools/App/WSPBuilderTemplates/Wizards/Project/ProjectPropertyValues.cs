/* Program : WSPBuilderTemplates
 * Created by: Tom Clarkson
 * Date : 2008
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace WSPBuilderTemplates
{

    /// <summary>
    /// Valid key values for properties added by a project wizard
    /// </summary>
    public class ProjectPropertyValues
    {
        /// <summary>
        /// Project Type GUIDs. Set to either C# or C# and workflow.
        /// </summary>
        public static string ProjectTypeGuids = "$projecttypeguids$";

        /// <summary>
        /// GUID for the WSP solution package.
        /// </summary>
        public static string SolutionID = "$solutionid$";

        /// <summary>
        /// Used by template project
        /// </summary>
        public static string VSSDKTargetsPath = "$vssdktargetspath$";

        /// <summary>
        /// The target Framework version of the project
        /// </summary>
        public static string TargetFrameworkVersion = "$targetframeworkversion$";
        
    }
}
