/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * Changes
 * ---------------------------------------------------------------------------- 
 * 20080221 TQC
 * Added SolutionID to substitutions
 * 
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
using WSPTools.BaseLibrary.IO;
using WSPTools.BaseLibrary.Security;
using WSPTools.BaseLibrary.VisualStudio;
using System.Reflection;
using VSLangProj80;
using VSLangProj;

namespace WSPBuilderTemplates
{
    /// <summary>
    /// Base class for all WSPBuilder item template wizards
    /// </summary>
    public abstract class BaseItemWizard : IWizard
    {
        private string AssemblyFullNameTemplate = @"{0}, Version={1}, Culture=neutral, PublicKeyToken={2}";
        private string AssemblyHandlerTemplate = Environment.NewLine + @"          {0}=""{1}, Version={2}, Culture=neutral, PublicKeyToken={3}""";
        private string ClassHandlerTemplate = Environment.NewLine + @"          {0}=""{1}.{2}""";


        private Dictionary<string, string> _Replacements = new Dictionary<string, string>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Replacements
        {
            get { return _Replacements; }
        }


        private Dictionary<string, ProjectItem> _itemRename = new Dictionary<string, ProjectItem>();
        public Dictionary<string, ProjectItem> ItemRename
        {
            get { return _itemRename; }
        }


        private Project _ActiveProject;
        public Project ActiveProject
        {
            get
            {
                if (_ActiveProject == null)
                {
                    Array activeSolutionProjects = this.Automation.ActiveSolutionProjects as Array;
                    if ((activeSolutionProjects != null) && (activeSolutionProjects.Length > 0))
                    {
                        _ActiveProject = activeSolutionProjects.GetValue(0) as Project;

                    }

                }
                return _ActiveProject;
            }
        }


        private DTE2 _Automation;
        public DTE2 Automation
        {
            get { return _Automation; }
        }


        private string _projectFullPath = null;
        public string ProjectFullPath
        {
            get 
            {
                if (String.IsNullOrEmpty(_projectFullPath))
                {
                    if (this.ActiveProject != null)
                    {
                        _projectFullPath = ActiveProject.Properties.Item("FullPath").Value as string;
                    }
                }
                return _projectFullPath; 
            }
            set { _projectFullPath = value; }
        }


        private VSProject2 _vsProject = null;
        public VSProject2 VsProject
        {
            get 
            {
                if (_vsProject == null)
                {
                    _vsProject = (VSProject2)this.ActiveProject.Object;
                }
                return _vsProject; 
            }
        }

        private References _projectReferences = null;
        public References ProjectReferences
        {
            get 
            {
                if (_projectReferences == null)
                {
                    _projectReferences = this.VsProject.References;
                }
                return _projectReferences; 
            }
        }


        private SelectedItem _selectedItem = null;
        public SelectedItem SelectedItem
        {
            get 
            {
                if (_selectedItem == null)
                {
                    foreach (SelectedItem si in this.Automation.SelectedItems)
                    {
                        _selectedItem = si;
                        break;
                    }
                }
                return _selectedItem; 
            }
            set { _selectedItem = value; }
        }



        #region IWizard Members



        public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            this._Replacements = replacementsDictionary;
            this._Automation = automationObject as DTE2;

            AddPublicKeyToken();

            // add aditional replacements - mostly details of the containing project
            this.Replacements.Add(ItemPropertyValues.FeatureID, Guid.NewGuid().ToString());
            this.Replacements.Add(ItemPropertyValues.ProjectName, this.ActiveProject.Name);
            this.Replacements.Add(ItemPropertyValues.RootNamespace, this.ActiveProject.Properties.Item("RootNamespace").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.DefaultNamespace, this.ActiveProject.Properties.Item("DefaultNamespace").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.ClassNamespace, this.ActiveProject.Properties.Item("DefaultNamespace").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.ClassName, string.Empty);
            this.Replacements.Add(ItemPropertyValues.OutputFilename, this.ActiveProject.Properties.Item("OutputFileName").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.AssemblyVersion, this.ActiveProject.Properties.Item("AssemblyVersion").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.FullPath, this.ActiveProject.Properties.Item("FullPath").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.AssemblyName, this.ActiveProject.Properties.Item("AssemblyName").Value.ToString());
            this.Replacements.Add(ItemPropertyValues.AssemblyHandlerAttribute, string.Empty);
            this.Replacements.Add(ItemPropertyValues.ClassHandlerAttribute, string.Empty);
            this.Replacements.Add(ItemPropertyValues.SharePoint2010Content, string.Empty);
            this.Replacements.Add(ItemPropertyValues.AssemblyFullName, CreateAssemblyFullName());
            this.Replacements.Add(ItemPropertyValues.FeatureTitle, this.Replacements[ItemPropertyValues.RootName]);
            this.Replacements.Add(ItemPropertyValues.FeatureDescription, this.Replacements[ItemPropertyValues.RootName]);
            this.Replacements.Add(ItemPropertyValues.FeatureActivatedContent, string.Empty);
            this.Replacements.Add(ItemPropertyValues.FeatureDeactivatingContent, string.Empty);
            this.Replacements.Add(ItemPropertyValues.FeatureUpgradingMethod, string.Empty);
            this.Replacements.Add(ItemPropertyValues.WebPartTitle, this.Replacements[ItemPropertyValues.RootName]);
            this.Replacements.Add(ItemPropertyValues.WebPartDescription, this.Replacements[ItemPropertyValues.RootName]);
            this.Replacements.Add(ItemPropertyValues.DelegateControlID, "PageHeader");
            
            ProjectPaths paths = new ProjectPaths(this.ProjectFullPath);
            this.Replacements.Add(ItemPropertyValues.SharePointRoot, paths.SharePointRootName);
            this.Replacements.Add(ItemPropertyValues.TargetPath, paths.SharePointRootName);

            EnsureSolutionID();
            ShowSettingsDialog();
            AddParameterReplacements();
        }

        public virtual bool ShouldAddProjectItem(string filePath)
        {
            Log("ShouldAddProjectItem: " + filePath);
            Log(this.ActiveProject);

            return true;
        }

        public virtual void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
            Log("BeforeOpeningFile: " + projectItem.Name + " Kind: "+projectItem.Kind);

        }


        /// <summary>
        /// Collect any settings needed before item generation
        /// </summary>
        public virtual void ShowSettingsDialog()
        {

        }

        /// <summary>
        /// Add any aditional parameters needed by subclasses.
        /// </summary>
        public virtual void AddParameterReplacements()
        {

        }


        public virtual void ProjectFinishedGenerating(EnvDTE.Project project)
        {

        }


        public virtual void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            Log("ProjectItemFinishedGenerating: " + projectItem.Name + " Kind: " + projectItem.Kind);
            //if (this.Replacements.ContainsKey(projectItem.Name))
            //{
            //    this.ItemRename.Add(projectItem.Name, projectItem);
            //}
        }

        public virtual void RunFinished()
        {
            Log("RunFinished");

            this.AddReferences();

            //foreach (KeyValuePair<string, ProjectItem> entry in this.ItemRename)
            //{
            //    ProjectItem projectItem = entry.Value;

            //    projectItem.Properties.Item("FileName").Value = this.Replacements[entry.Key];
            //}
        }

        public virtual void AddReferences()
        {
            Log("AddReferences");
        }


        public void AddReference(string name)
        {
            try
            {
                this.ProjectReferences.Add(name);
            }
            catch 
            {
                MessageBox.Show("Unable to add the reference " + name + " to the project. Please try to add it yourself if possible.", "Reference error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AddPublicKeyToken()
        {
            // Get snk file
            string path = Path.Combine(this.ProjectFullPath, ActiveProject.Properties.Item("AssemblyOriginatorKeyFile").Value as string);

            string publicKeyToken = StrongNameKey.GetPublicKeyToken(path);
            if (!String.IsNullOrEmpty(publicKeyToken))
            {
                this.Replacements.Add(ItemPropertyValues.PublicKeyToken, publicKeyToken);
            }
            else
            {
                throw new ApplicationException("There is no strong name key assigned to this project. Please add one.");
            }
        }


        private void EnsureSolutionID()
        {
            string solutionId = Guid.NewGuid().ToString();
            if (!ManifestExtensions.DoManifestExits(this.ProjectFullPath))
            {
                ManifestExtensions.CreateManifestFile(this.ProjectFullPath, solutionId);
            }
            else
            {
                solutionId = ManifestExtensions.GetSolutionID(this.ProjectFullPath);
            }

            this.Replacements.Add(ItemPropertyValues.SolutionID, solutionId);

        }


        /// <summary>
        /// Creates a string with the following format:
        /// AttributeName="$AssemblyName$, Version=$AssemblyVersion$, Culture=neutral, PublicKeyToken=$PublicKeyToken$"
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        protected string CreateAssemblyHandlerString(string attributeName)
        {
            return string.Format(AssemblyHandlerTemplate, 
                attributeName, 
                this.Replacements[ItemPropertyValues.AssemblyName],
                this.Replacements[ItemPropertyValues.AssemblyVersion],
                this.Replacements[ItemPropertyValues.PublicKeyToken]
                );
        }

        protected string CreateClassHandlerString(string attributeName, string classNamespace, string className)
        {
            return string.Format(ClassHandlerTemplate, attributeName, classNamespace, className);
        }

        private string CreateAssemblyFullName()
        {
            return string.Format(AssemblyFullNameTemplate,
                this.Replacements[ItemPropertyValues.AssemblyName],
                this.Replacements[ItemPropertyValues.AssemblyVersion],
                this.Replacements[ItemPropertyValues.PublicKeyToken]
                );
        }

        //protected string GetCaseSensitivePath(string targetPath)
        //{
        //    string result = targetPath;

        //    string fullPath = Path.Combine(this.ProjectFullPath, targetPath);
        //    DirectoryInfo dir = new DirectoryInfo(fullPath);
        //    if (dir.Exists)
        //    {
        //        result = dir.FullName.Substring(this.ProjectFullPath.Length); 
        //    }

        //    return result;
        //}


        #region Loggin


        public void Log(string message)
        {
#if DEBUG
            //MessageBox.Show(message);
#endif
        }

        public void Log(Project project)
        {
#if DEBUG
            //StringBuilder sb = new StringBuilder();
            //foreach (Property property in project.Properties)
            //{
            //    try
            //    {
            //        sb.Append(property.Name + "=" + property.Value+"\r\n");

            //    }
            //    catch 
            //    {
            //    }
            //}
            //MessageBox.Show(sb.ToString());
#endif
        }

        #endregion 

        #endregion
    }
}
