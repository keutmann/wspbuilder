using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace $DefaultNamespace$
{
    public class $rootname$Control : BaseFieldControl
    {
                private $rootname$ field;
        private ListBox listBox;

        public $rootname$Control($rootname$ parentField)
        {
            this.field = parentField;
            this.listBox = new ListBox();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls(); 
listBox = new ListBox();
this.Controls.Add(listBox);
        }

        public override void UpdateFieldValueInItem()
        {
            this.EnsureChildControls();

            try
            {
                this.Value = this.listBox.SelectedValue;
                this.ItemFieldValue = this.Value;
            }
            catch (Exception)
            {
                ;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);
        }
    }
}

