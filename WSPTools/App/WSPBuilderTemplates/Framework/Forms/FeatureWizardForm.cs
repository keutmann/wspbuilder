using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WSPBuilderTemplates.Framework.Forms
{
    public partial class FeatureWizardForm : BaseWizard
    {
        //public FeatureWizardForm() : base()
        //{
        //    InitializeComponent();
        //}

        public FeatureWizardForm(Dictionary<string, string> replacements)
            : base(replacements)
        {
            InitializeComponent();
            this.Feature.InitializeControls(this.Replacements);
        }


        public override void Save(Dictionary<string, string> replacements)
        {
            this.Feature.Save(replacements);
        }
    }
}
