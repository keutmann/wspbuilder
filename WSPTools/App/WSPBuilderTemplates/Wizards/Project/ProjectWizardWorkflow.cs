/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * Modified by Carsten Keutmann
 * Date : 2009
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace WSPBuilderTemplates
{
    /// <summary>
    /// Project Wizard With project type guid changed to support workflow
    /// </summary>
    public class ProjectWizardWorkflow : ProjectWizard
    {

        #region IWizard Members

        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
            
            // change project type guid to c# and workflow
            // Workflow C#: {14822709-B5A1-4724-98CA-57A101D1B079}
            // Windows C# : {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
            replacementsDictionary[ProjectPropertyValues.ProjectTypeGuids] = "{14822709-B5A1-4724-98CA-57A101D1B079};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        }

        #endregion
    }
}
