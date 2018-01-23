namespace WSPBuilderTemplates.Framework.Forms
{
    partial class FeatureWizardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wizardController1 = new WSPBuilderTemplates.Framework.Wizard.WizardController();
            this.wizardPage1 = new WSPBuilderTemplates.Framework.Wizard.WizardPage();
            this.Feature = new WSPBuilderTemplates.Framework.UserControls.FeaturePage();
            this.wizardController1.SuspendLayout();
            this.wizardPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardController1
            // 
            this.wizardController1.Controls.Add(this.wizardPage1);
            this.wizardController1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardController1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizardController1.Location = new System.Drawing.Point(0, 0);
            this.wizardController1.Name = "wizardController1";
            this.wizardController1.Pages.AddRange(new WSPBuilderTemplates.Framework.Wizard.WizardPage[] {
            this.wizardPage1});
            this.wizardController1.Size = new System.Drawing.Size(607, 373);
            this.wizardController1.TabIndex = 0;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Controls.Add(this.Feature);
            this.wizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage1.IsFinishPage = false;
            this.wizardPage1.Location = new System.Drawing.Point(0, 0);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(607, 325);
            this.wizardPage1.TabIndex = 1;
            // 
            // Feature
            // 
            this.Feature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Feature.Location = new System.Drawing.Point(0, 0);
            this.Feature.Name = "Feature";
            this.Feature.Size = new System.Drawing.Size(607, 325);
            this.Feature.TabIndex = 0;
            // 
            // FeatureWizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 373);
            this.Controls.Add(this.wizardController1);
            this.Name = "FeatureWizardForm";
            this.Text = "Feature Wizard";
            this.wizardController1.ResumeLayout(false);
            this.wizardPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WSPBuilderTemplates.Framework.Wizard.WizardController wizardController1;
        private WSPBuilderTemplates.Framework.Wizard.WizardPage wizardPage1;
        public WSPBuilderTemplates.Framework.UserControls.FeaturePage Feature;


    }
}