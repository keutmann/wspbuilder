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

namespace WSPBuilderTemplates
{
    /// <summary>
    /// Wizard for a feature item template. Includes a feature settings dialog and renames
    /// the feature folder to match the feature name.
    /// May be overridden for more complex feature item templates.
    /// </summary>
    public class ItemTemplateWizard : BaseItemWizard
    {
        /// <summary>
        /// used to store a reference to the feature folder
        /// </summary>
        private ProjectItem folderItem;


        public ItemTemplateWizard()
        {
        }

        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            if (projectItem == null) return;

            base.ProjectItemFinishedGenerating(projectItem);
            if (projectItem.Name == "NewItemTemplate")
            {
                folderItem = projectItem;
            }
            if (projectItem.Properties.Item("Filename").Value != null
                && projectItem.Properties.Item("Filename").Value.ToString().EndsWith("._vstemplate"))
            {
                projectItem.Properties.Item("Filename").Value = projectItem.Properties.Item("Filename").Value.ToString().Replace("._vstemplate", ".vstemplate");
            }

            // get a reference to the feature folder
            if (projectItem.Collection != null && projectItem.Collection.Parent != null && ((ProjectItem)projectItem.Collection.Parent).Name == "NewItemTemplate")
                folderItem = (ProjectItem)projectItem.Collection.Parent;
        }

        public override void RunFinished()
        {
            base.RunFinished();

            // rename the feature folder to match the name of the feature as VS
            // does not support substitution in folder names
            if (folderItem != null && Replacements.ContainsKey(ItemPropertyValues.RootName))
            {
                folderItem.Properties.Item("FileName").Value = Replacements[ItemPropertyValues.RootName];
            }

        }


    }
}
