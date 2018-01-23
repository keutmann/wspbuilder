using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace $DefaultNamespace$
{
    public class $rootname$FieldEditor : UserControl, IFieldEditor
    {
        // Fields
        protected DropDownList DdlLookupFieldTargetList;
        private $rootname$ fld$rootname$;
        protected Label LabelLookupFieldTargetListTitle;

        public void InitializeWithField(SPField field)
        {
            this.fld$rootname$ = field as $rootname$;

            if (this.Page.IsPostBack)
            {
                return;
            }

            DdlLookupFieldTargetList.Items.Clear();

            DdlLookupFieldTargetList.Items.Add("One");
            DdlLookupFieldTargetList.Items.Add("Two");
            DdlLookupFieldTargetList.Items.Add("Default Value");
            DdlLookupFieldTargetList.Items.Add("Four");


            if (field != null)
            {
                DdlLookupFieldTargetList.SelectedValue = fld$rootname$.MyCustomProperty;
                this.DdlLookupFieldTargetList.Visible = true;
            }
            else
            {
                DdlLookupFieldTargetList.SelectedValue = "Default Value";
                this.DdlLookupFieldTargetList.Visible = true;
            }
        }

        public void OnSaveChange(SPField field, bool bNewField)
        {
            $rootname$ lookup = ($rootname$)field;

            lookup.IsNew = bNewField;
            lookup.MyCustomProperty = this.DdlLookupFieldTargetList.SelectedValue;

        }


        // Properties
        public bool DisplayAsNewSection
        {
            get
            {
                return false;
            }
        }



    }

}