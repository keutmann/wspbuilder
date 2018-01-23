using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Security;
using Keutmann.Framework;

namespace WSPBuilderTemplates.Framework.UserControls
{
    public partial class WebPartPage : BasePage
    {
        public WebPartPage()
        {
            InitializeComponent();
        }

        public override void InitializeControls(Dictionary<string, string> replacements)
        {
            this.tbTitle.Text = StringExtensions.UnescapeXml(replacements[ItemPropertyValues.WebPartTitle]);
            this.tbDescription.Text = StringExtensions.UnescapeXml("Description for " + replacements[ItemPropertyValues.WebPartDescription]);
            this.cbRemoveWebpart.Checked = true;
        }

        public override void Save(Dictionary<string, string> replacements)
        {
            replacements[ItemPropertyValues.WebPartTitle] = StringExtensions.EscapeXml(this.tbTitle.Text);
            replacements[ItemPropertyValues.WebPartDescription] = StringExtensions.EscapeXml(this.tbDescription.Text);
        }
    }
}
