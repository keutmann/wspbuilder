using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Keutmann.Framework;

namespace WSPBuilderTemplates.Framework.UserControls
{
    public partial class FeaturePage : BasePage
    {
        public FeaturePage()
        {
            InitializeComponent();
        }

        public override void InitializeControls(Dictionary<string, string> replacements)
        {
            this.tbTitle.Text = StringExtensions.UnescapeXml(replacements[ItemPropertyValues.FeatureTitle]);
            this.tbDescription.Text = StringExtensions.UnescapeXml("Description for " + replacements[ItemPropertyValues.FeatureTitle]);
            this.dlScope.Text = "Site";
            this.cbEventHandler.Checked = true;
        }

        public override void Save(Dictionary<string, string> replacements)
        {
            replacements[ItemPropertyValues.FeatureTitle] = StringExtensions.EscapeXml(this.tbTitle.Text);
            replacements[ItemPropertyValues.FeatureDescription] = StringExtensions.EscapeXml(this.tbDescription.Text);
            replacements[ItemPropertyValues.FeatureScope] = this.dlScope.Text;
        }


    }
}
