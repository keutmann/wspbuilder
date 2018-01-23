namespace WSPBuilderTemplates.Framework.UserControls
{
    partial class WebPartPage
    {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebPartPage));
            this.cbRemoveWebpart = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.header1 = new WSPBuilderTemplates.Framework.Wizard.Header();
            this.lblWebPartRemoval = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbRemoveWebpart
            // 
            this.cbRemoveWebpart.AutoSize = true;
            this.cbRemoveWebpart.Location = new System.Drawing.Point(98, 181);
            this.cbRemoveWebpart.Name = "cbRemoveWebpart";
            this.cbRemoveWebpart.Size = new System.Drawing.Size(364, 17);
            this.cbRemoveWebpart.TabIndex = 14;
            this.cbRemoveWebpart.Text = "Remove the Web Part from the gallery when the Feature is deactivated.";
            this.cbRemoveWebpart.UseVisualStyleBackColor = true;
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
            // header1
            // 
            this.header1.BackColor = System.Drawing.SystemColors.Control;
            this.header1.CausesValidation = false;
            this.header1.Description = "Configure the Web Part settings";
            this.header1.Dock = System.Windows.Forms.DockStyle.Top;
            this.header1.Image = ((System.Drawing.Image)(resources.GetObject("header1.Image")));
            this.header1.Location = new System.Drawing.Point(0, 0);
            this.header1.Name = "header1";
            this.header1.Size = new System.Drawing.Size(608, 64);
            this.header1.TabIndex = 15;
            this.header1.Title = "Web Part";
            // 
            // lblWebPartRemoval
            // 
            this.lblWebPartRemoval.AutoSize = true;
            this.lblWebPartRemoval.Location = new System.Drawing.Point(16, 182);
            this.lblWebPartRemoval.Name = "lblWebPartRemoval";
            this.lblWebPartRemoval.Size = new System.Drawing.Size(76, 13);
            this.lblWebPartRemoval.TabIndex = 16;
            this.lblWebPartRemoval.Text = "Removal code";
            // 
            // WebPartPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblWebPartRemoval);
            this.Controls.Add(this.header1);
            this.Controls.Add(this.cbRemoveWebpart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.tbTitle);
            this.Name = "WebPartPage";
            this.Size = new System.Drawing.Size(608, 255);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox cbRemoveWebpart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private WSPBuilderTemplates.Framework.Wizard.Header header1;
        private System.Windows.Forms.Label lblWebPartRemoval;
        public System.Windows.Forms.TextBox tbDescription;
        public System.Windows.Forms.TextBox tbTitle;
    }
}
