/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
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
using Microsoft.Win32;

namespace WSPBuilderTemplates
{
    /// <summary>
    /// Project Wizard With additional substitution for VSSDK targets
    /// </summary>
    public class ProjectWizardTemplate : ProjectWizard
    {




        #region IWizard Members



        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
            string path = null;
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\8.0\MSBuild\SafeImports");
            if (rk != null)
            {
                foreach (string vn in rk.GetValueNames())
                {
                    string s = rk.GetValue(vn, "") as string;
                    if (s.EndsWith("microsoft.vssdk.targets", StringComparison.InvariantCultureIgnoreCase))
                    {
                        path = s;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\9.0\MSBuild\SafeImports");
                if (rk != null)
                {
                    foreach (string vn in rk.GetValueNames())
                    {
                        string s = rk.GetValue(vn, "") as string;
                        if (s.EndsWith("microsoft.vssdk.targets", StringComparison.InvariantCultureIgnoreCase))
                        {
                            path = s;
                            break;
                        }
                    }
                }

            }
            if (string.IsNullOrEmpty(path))
            {
                path = @"C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Tools\Build\Microsoft.VsSDK.targets";
            }


            replacementsDictionary[ProjectPropertyValues.VSSDKTargetsPath] = path;
        }

        #endregion
    }
}
