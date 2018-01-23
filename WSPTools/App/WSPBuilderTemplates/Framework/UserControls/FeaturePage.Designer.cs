namespace WSPBuilderTemplates.Framework.UserControls
{
    partial class FeaturePage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeaturePage));
            this.cbEventHandler = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.dlScope = new System.Windows.Forms.ComboBox();
            this.header1 = new WSPBuilderTemplates.Framework.Wizard.Header();
            this.SuspendLayout();
            // 
            // cbEventHandler
            // 
            this.cbEventHandler.AutoSize = true;
            this.cbEventHandler.Location = new System.Drawing.Point(22, 208);
            this.cbEventHandler.Name = "cbEventHandler";
            this.cbEventHandler.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbEventHandler.Size = new System.Drawing.Size(92, 17);
            this.cbEventHandler.TabIndex = 14;
            this.cbEventHandler.Text = "Event handler";
            this.cbEventHandler.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Scope";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Title";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(98, 111);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(498, 64);
            this.tbDescription.TabIndex = 10;
            // 
            // tbTitle
            // 
            this.tbTitle.Location = new System.Drawing.Point(98, 85);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(498, 20);
            this.tbTitle.TabIndex = 9;
            // 
            // dlScope
            // 
            this.dlScope.FormattingEnabled = true;
            this.dlScope.Items.AddRange(new object[] {
            "Web",
            "Site",
            "WebApplication",
            "Farm"});
            this.dlScope.Location = new System.Drawing.Point(98, 181);
            this.dlScope.Name = "dlScope";
            this.dlScope.Size = new System.Drawing.Size(498, 21);
            this.dlScope.TabIndex = 8;
            // 
            // header1
            // 
            this.header1.BackColor = System.Drawing.SystemColors.Control;
            this.header1.CausesValidation = false;
            this.header1.Description = "Configure the feature settings";
            this.header1.Dock = System.Windows.Forms.DockStyle.Top;
            this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
            this.header1.Location = new System.Drawing.Point(0, 0);
            this.header1.Name = "header1";
            this.header1.Size = new System.Drawing.Size(608, 64);
            this.header1.TabIndex = 15;
            this.header1.Title = "Featue";
            // 
            // FeaturePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.header1);
            this.Controls.Add(this.cbEventHandler);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.dlScope);
            this.Name = "FeaturePage";
            this.Size = new System.Drawing.Size(608, 255);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox cbEventHandler;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private WSPBuilderTemplates.Framework.Wizard.Header header1;
        public System.Windows.Forms.TextBox tbDescription;
        public System.Windows.Forms.TextBox tbTitle;
        public System.Windows.Forms.ComboBox dlScope;
    }
}
