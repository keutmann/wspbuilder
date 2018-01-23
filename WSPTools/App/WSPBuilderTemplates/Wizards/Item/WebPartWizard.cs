/* Program : WSPBuilderTemplates
 * Created by: Carsten Keutmann
 * Date : 2009
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using WSPTools.BaseLibrary.Win32;
using WSPTools.BaseLibrary.IO;
using System.Reflection;
using System.Windows.Forms;
using WSPBuilderTemplates.Framework.Forms;

namespace WSPBuilderTemplates
{

    public class WebPartWizard : DefaultFeatureWizard
    {
        public string WebPartReceiverDeactivateName = "WebPartReceiverDeactivate.txt";
        public string WebPartReceiverDeactivateTemplate { get; set; }

        public bool AddRemovalCode { get; set; }

        public WebPartWizard()
        {

        }

        public override void AddParameterReplacements()
        {
            base.AddParameterReplacements();

            if (this.AddRemovalCode)
            {
                this.WebPartReceiverDeactivateTemplate = ResourceReader.ReadString(Assembly.GetExecutingAssembly(), "WSPBuilderTemplates", this.WebPartReceiverDeactivateName);
                // Combine the code templates
                this.Replacements[ItemPropertyValues.FeatureDeactivatingContent] = this.ReceiverCodeTemplate.Replace(ItemPropertyValues.CodeContent, this.WebPartReceiverDeactivateTemplate);
            }
        }

        /// <summary>
        /// Display a dialog to collect common feature settings
        /// </summary>
        public override void ShowSettingsDialog()
        {
            WebPartWizardForm form = new WebPartWizardForm(this.Replacements);

            if (form.ShowDialog() == DialogResult.OK)
            {
                form.Save(this.Replacements);

                this.AddRemovalCode = form.WebPart.cbRemoveWebpart.Checked;
                this.AddFeatureReceiver = (this.AddRemovalCode) ? true : form.Feature.cbEventHandler.Checked;
            }
            else
            {
                throw new ApplicationException("User cancelled wizard!");
            }

            Application.DoEvents();
        }


    }
}
