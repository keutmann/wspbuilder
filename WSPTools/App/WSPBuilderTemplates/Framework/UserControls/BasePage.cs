using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WSPBuilderTemplates.Framework.UserControls
{
    public class BasePage : UserControl
    {
        public BasePage()
            : base()
        {
        }

        public virtual void InitializeControls(Dictionary<string, string> replacements)
        {
        }

        public virtual void Save(Dictionary<string, string> replacements)
        {
        }

    }
}
