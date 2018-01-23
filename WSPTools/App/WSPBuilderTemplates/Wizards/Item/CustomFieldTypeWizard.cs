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
using System.Windows.Forms;

namespace WSPBuilderTemplates
{
    /// <summary>
    /// Wizard for a feature item template. Includes a feature settings dialog and renames
    /// the feature folder to match the feature name.
    /// May be overridden for more complex feature item templates.
    /// </summary>
    public class CustomFieldTypeWizard : BaseItemWizard
    {


        public CustomFieldTypeWizard()
        {
        }


        public override bool ShouldAddProjectItem(string filePath)
        {
            //// exclude the custom control if it is not used.
            //if (filePath.EndsWith("FieldEditor.ascx") || filePath.EndsWith("FieldEditor.cs"))
            //{
            //    return useFieldEditorUserControl;
            //}

            return base.ShouldAddProjectItem(filePath);


        }

        /// <summary>
        /// Display a dialog to collect settings
        /// </summary>
        public override void ShowSettingsDialog()
        {
            Replacements.Add(ItemPropertyValues.FieldEditorUserControl, "<Field Name=\"FieldEditorUserControl\">/_controltemplates/$rootname$FieldEditor.ascx</Field>");
            Replacements.Add(ItemPropertyValues.HideCustomProperty, "TRUE");

            //DialogResult dr = MessageBox.Show("Do you want to use a custom control on the edit field page? Due to a SharePoint bug (http://support.microsoft.com/kb/932055), this is required if the 12/4/2007 hotfix is not applied.", "Use custom field editor?", MessageBoxButtons.YesNo);
            //useFieldEditorUserControl = dr == DialogResult.Yes;

            //if (!useFieldEditorUserControl)
            //{
            //    Replacements.Add("$FieldEditorUserControl$", "<!-- A SharePoint bug (http://support.microsoft.com/kb/932055) may prevent the custom properties below working. Use a custom field editor control or see http://thomascarpe.com/Lists/Posts/Post.aspx?ID=25 for an alternative workaround. -->");
            //    Replacements.Add("$HideCustomProperty$", "FALSE");
            //}
            //else
            //{
            //    Replacements.Add("$FieldEditorUserControl$", "<Field Name=\"FieldEditorUserControl\">/_controltemplates/$rootname$FieldEditor.ascx</Field>");
            //    Replacements.Add("$HideCustomProperty$", "TRUE");
            //}

        }


    }
}
