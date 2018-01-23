namespace WSPBuilderTemplates.Framework.Forms
{
    partial class WebPartWizardForm
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
            this.wizardPage2 = new WSPBuilderTemplates.Framework.Wizard.WizardPage();
            this.WebPart = new WSPBuilderTemplates.Framework.UserControls.WebPartPage();
            this.wizardController1.SuspendLayout();
            this.wizardPage1.SuspendLayout();
            this.wizardPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardController1
            // 
            this.wizardController1.Controls.Add(this.wizardPage2);
            this.wizardController1.Controls.Add(this.wizardPage1);
            this.wizardController1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardController1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizardController1.Location = new System.Drawing.Point(0, 0);
            this.wizardController1.Name = "wizardController1";
            this.wizardController1.Pages.AddRange(new WSPBuilderTemplates.Framework.Wizard.WizardPage[] {
            this.wizardPage1,
            this.wizardPage2});
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
            // wizardPage2
            // 
            this.wizardPage2.Controls.Add(this.WebPart);
            this.wizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage2.IsFinishPage = false;
            this.wizardPage2.Location = new System.Drawing.Point(0, 0);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.Size = new System.Drawing.Size(607, 325);
            this.wizardPage2.TabIndex = 2;
            // 
            // WebPart
            // 
            this.WebPart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebPart.Location = new System.Drawing.Point(0, 0);
            this.WebPart.Name = "WebPart";
            this.WebPart.Size = new System.Drawing.Size(607, 325);
            this.WebPart.TabIndex = 0;
            // 
            // WebPartWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 373);
            this.Controls.Add(this.wizardController1);
            this.Name = "WebPartWizard";
            this.Text = "WebPart Wizard";
            this.wizardController1.ResumeLayout(false);
            this.wizardPage1.ResumeLayout(false);
            this.wizardPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WSPBuilderTemplates.Framework.Wizard.WizardController wizardController1;
        private WSPBuilderTemplates.Framework.Wizard.WizardPage wizardPage1;
        private WSPBuilderTemplates.Framework.Wizard.WizardPage wizardPage2;
        public WSPBuilderTemplates.Framework.UserControls.FeaturePage Feature;
        public WSPBuilderTemplates.Framework.UserControls.WebPartPage WebPart;
    }
}