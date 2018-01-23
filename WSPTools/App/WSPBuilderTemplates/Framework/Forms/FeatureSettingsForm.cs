/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * Modified by Carsten Keutmann
 * Date : 2009
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WSPTools.BaseLibrary.VisualStudio;

namespace WSPBuilderTemplates
{
    public partial class FeatureSettingsForm : Form
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


        public FeatureSettingsForm(Dictionary<string, string> replacements)
        {
            InitializeComponent();
            this.Replacements = replacements;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validate the Name!
            //string projectPath = this.Replacements[ItemPropertyValues.FullPath];
            //ProjectPaths paths = new ProjectPaths(projectPath);
            //string featureXmlPath = paths.PathFeatures + this.FeatureTitle+ @"\Feature.xml";
            //if (File.Exists(featureXmlPath))
            //{
            //    MessageBox.Show("The feature " + this.FeatureTitle + " already exist. Please specify an other name.", "Feature already exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else
            //{
            //    this.Close();
            //}
            this.Close();
        }


        public string FeatureTitle
        {
            get { return txtTitle.Text; }
            set { txtTitle.Text = value; }
        }

        public string FeatureDesc
        {
            get { return txtDesc.Text; }
            set { txtDesc.Text = value; }
        }

        public string FeatureScope
        {
            get { return cbScope.Text as string; }
            set { cbScope.Text = value; }
        }


    }
}