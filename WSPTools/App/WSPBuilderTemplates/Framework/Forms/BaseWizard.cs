using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WSPBuilderTemplates.Framework.Forms
{
    public partial class BaseWizard : Form
    {
        private Dictionary<string, string> _replacements = new Dictionary<string, string>();
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Replacements
        {
            get { return _replacements; }
            set { _replacements = value; }
        }

        public BaseWizard()
        {
            InitializeComponent();
        }

        public BaseWizard(Dictionary<string, string> replacements) : this()
        {
            this.Replacements = replacements;
        }

        public virtual void Save(Dictionary<string, string> replacements)
        {
        }
             
    }
}
