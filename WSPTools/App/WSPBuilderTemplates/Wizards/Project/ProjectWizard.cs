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
using VSLangProj;
using VSLangProj2;
using VSLangProj80;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Windows.Forms;
using WSPTools.BaseLibrary.Win32;
using WSPTools.BaseLibrary.IO;
using WSPTools.BaseLibrary.VisualStudio;
using WSPTools.BaseLibrary.Security;

// VS2008 Class Library ProjectGuid: D29B2348-DE8A-4635-AD69-20ACB8A8BBD7
// VS2010 ClassLibrary Project type: 34ECBB52-A461-4F27-8F63-1D858A4821D9


namespace WSPBuilderTemplates
{
    public class ProjectWizard : IWizard
    {

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Replacements
        {
            get { return _Replacements; }
        }
        private Dictionary<string, string> _Replacements = new Dictionary<string, string>();


        protected DTE2 Automation
        {
            get { return _Automation; }
        }
        private DTE2 _Automation;

         #region IWizard Members
        public virtual void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
            Log("BeforeOpeningFile: " + projectItem.Name);
        }



        public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            Log("RunStarted");
            this._Replacements = replacementsDictionary;
            this._Automation = automationObject as DTE2;

            replacementsDictionary.Add(ProjectPropertyValues.SolutionID, Guid.NewGuid().ToString());
            // c# only
            // WebApplication: {349C5851-65DF-11DA-9384-00065B846F21}
            // Windows C# : {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
            // Format: {349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}
            replacementsDictionary.Add(ProjectPropertyValues.ProjectTypeGuids, "{349C5851-65DF-11DA-9384-00065B846F21};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
                                                                                 

            Log("RunStarted finished!");
        }

        public virtual bool ShouldAddProjectItem(string filePath)
        {
            Log("ShouldAddProjectItem: " + filePath);

            bool result = true;

            return result;
        }

        public virtual void ProjectFinishedGenerating(EnvDTE.Project project)
        {

            Log("ProjectFinishedGenerating");

            string projectPath = (string)project.Properties.Item("FullPath").Value;

            string pkt = StrongNameKey.CreateKeyPairFile(projectPath + project.Name + ".snk", 1024);
            project.ProjectItems.AddFromFile(projectPath + project.Name + ".snk");

            string solutionID = this.Replacements[ProjectPropertyValues.SolutionID];
            string filename = ManifestExtensions.CreateManifestFile(projectPath, solutionID);
            
            // Add the new file to the project
            project.ProjectItems.AddFromFile(filename);

         
            Log("ProjectFinishedGenerating Finished");

        }




        public virtual void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
            Log("ProjectItemFinishedGenerating: " + projectItem.Name + " Kind: " +projectItem.Kind);

        }

        public virtual void RunFinished()
        {
            Log("RunFinished");

        }

        private void Log(string message)
        {
#if DEBUG
            //MessageBox.Show(message);
#endif
        }

        #endregion
    }
}
