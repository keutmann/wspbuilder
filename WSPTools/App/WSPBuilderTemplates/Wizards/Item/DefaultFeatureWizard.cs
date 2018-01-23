/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
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
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Windows.Forms;
using WSPTools.BaseLibrary.VisualStudio;
using WSPTools.BaseLibrary.IO;
using System.Reflection;
using Keutmann.Framework;
using WSPTools.BaseLibrary.Win32;
using WSPBuilderTemplates.Framework.Forms;

namespace WSPBuilderTemplates
{
    /// <summary>
    /// Wizard for a feature item template. Includes a feature settings dialog and renames
    /// the feature folder to match the feature name.
    /// May be overridden for more complex feature item templates.
    /// </summary>
    public class DefaultFeatureWizard : BaseItemWizard
    {
        public bool AddFeatureReceiver { get; set; }

        
        //ReceiverAssembly="$AssemblyName$, Version=$AssemblyVersion$, Culture=neutral, PublicKeyToken=$PublicKeyToken$"
        //ReceiverClass="$DefaultNamespace$.EventHandlers.Features.$rootname$"


        public string ReceiverCodeName { get; set; }
        public string ReceiverCodeTemplate { get; set; }
        public string UpgradingMethodName = "FeatureReceiverUpgradingMethod.txt";
        public string UpgradingMethodTemplate { get; set; }


        public DefaultFeatureWizard()
        {
        }

        public override void AddParameterReplacements()
        {
            base.AddParameterReplacements();

            string targetPath = this.Replacements[ItemPropertyValues.SharePointRoot] + @"\Template\Features\" + this.Replacements[ItemPropertyValues.RootName];
            this.Replacements[ItemPropertyValues.TargetPath] = targetPath;
            this.Replacements[ItemPropertyValues.FeatureName] = this.Replacements[ItemPropertyValues.RootName];

            if (this.AddFeatureReceiver)
            {
                this.Replacements[ItemPropertyValues.AssemblyHandlerAttribute] = this.CreateAssemblyHandlerString("ReceiverAssembly");
                this.Replacements[ItemPropertyValues.ClassHandlerAttribute] = this.CreateClassHandlerString(
                    "ReceiverClass",
                    this.Replacements[ItemPropertyValues.RootNamespace] + ".EventHandlers.Features",
                    this.Replacements[ItemPropertyValues.RootName]+"Receiver");

                this.ReceiverCodeName = String.Format("FeatureReceiverScope{0}.txt", this.Replacements[ItemPropertyValues.FeatureScope]);
                this.ReceiverCodeTemplate = ResourceReader.ReadString(Assembly.GetExecutingAssembly(), "WSPBuilderTemplates", this.ReceiverCodeName);

                string receiverCode = StringExtensions.CommentCode(this.ReceiverCodeTemplate.Replace(ItemPropertyValues.CodeContent, string.Empty));
                this.Replacements[ItemPropertyValues.FeatureActivatedContent] = receiverCode;
                this.Replacements[ItemPropertyValues.FeatureDeactivatingContent] = receiverCode;

                if (SharePointRegistry.IsSharePoint14)
                {
                    this.UpgradingMethodTemplate = ResourceReader.ReadString(Assembly.GetExecutingAssembly(), "WSPBuilderTemplates", this.UpgradingMethodName);
                    this.Replacements[ItemPropertyValues.FeatureUpgradingMethod] = this.UpgradingMethodTemplate.Replace(ItemPropertyValues.CodeContent, string.Empty);
                }
            }
        }


        /// <summary>
        /// Display a dialog to collect common feature settings
        /// </summary>
        public override void ShowSettingsDialog()
        {
            FeatureWizardForm form = new FeatureWizardForm(this.Replacements);

            if (form.ShowDialog() == DialogResult.OK)
            {
                form.Save(this.Replacements);

                this.AddFeatureReceiver = form.Feature.cbEventHandler.Checked;
            }

            Application.DoEvents();
        }


        public override bool ShouldAddProjectItem(string filePath)
        {
            bool result = base.ShouldAddProjectItem(filePath);

            if (!this.AddFeatureReceiver && filePath.Equals("ClassReceiver.cs", StringComparison.OrdinalIgnoreCase))
            {
                result = false;
            }

            return result;
        }

        public override void AddReferences()
        {
            base.AddReferences();
            // Get a reference to the Visual Studio Project and use it to add a reference to the XML assembly
            this.AddReference("Microsoft.SharePoint");
        }

        //private string GetDefaultFeatureName()
        //{
        //    string result = string.Empty;
        //    string projectPath = this.Replacements[ItemPropertyValues.FullPath];
        //    ProjectPaths paths = new ProjectPaths(projectPath);

        //    int counter = 1;
        //    string path = string.Empty;
        //    while (string.IsNullOrEmpty(path) && counter < 1000)
        //    {
        //        result = Replacements[ItemPropertyValues.RootName] + counter;
        //        path = paths.PathFeatures + result;
        //        if (Directory.Exists(result))
        //        {
        //            path = string.Empty;
        //        }
        //        counter++;
        //    }

        //    return result;
        //}

    }
}
