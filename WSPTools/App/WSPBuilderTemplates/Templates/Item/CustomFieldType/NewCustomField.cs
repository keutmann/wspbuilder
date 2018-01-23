using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace $DefaultNamespace$
{
    public class $rootname$ : SPFieldText
    {
            private static string[] CustomPropertyNames = new string[] { "MyCustomProperty" };

            public $rootname$(SPFieldCollection fields, string fieldName)
            : base(fields, fieldName)
        {
            InitProperties();
        }

        public $rootname$(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
            InitProperties();
        }

        #region Property storage and bug workarounds - do not edit

        /// <summary>
        /// Indicates that the field is being created rather than edited. This is necessary to 
        /// work around some bugs in field creation.
        /// </summary>
        public bool IsNew
        {
            get { return _IsNew; }
            set { _IsNew = value; }
        }
        private bool _IsNew = false;

        /// <summary>
        /// Backing fields for custom properties. Using a dictionary to make it easier to abstract
        /// details of working around SharePoint bugs.
        /// </summary>
        private Dictionary<string, string> CustomProperties = new Dictionary<string, string>();

        /// <summary>
        /// Static store to transfer custom properties between instances. This is needed to allow
        /// correct saving of custom properties when a field is created - the custom property 
        /// implementation is not used by any out of box SharePoint features so is really buggy.
        /// </summary>
        private static Dictionary<string, string> CustomPropertiesForNewFields = new Dictionary<string, string>();

        /// <summary>
        /// Initialise backing fields from base property store
        /// </summary>
        private void InitProperties()
        {
            foreach (string propertyName in CustomPropertyNames)
            {
                CustomProperties[propertyName] = base.GetCustomProperty(propertyName) + "";
            }
        }

        /// <summary>
        /// Take properties from either the backing fields or the static store and 
        /// put them in the base property store
        /// </summary>
        private void SaveProperties()
        {
            foreach (string propertyName in CustomPropertyNames)
            {
                base.SetCustomProperty(propertyName, GetCustomProperty(propertyName));
            }
        }

        /// <summary>
        /// Get an identifier for the field being added/edited that will be unique even if
        /// another user is editing a property of the same name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private string GetCacheKey(string propertyName)
        {
            return SPContext.Current.GetHashCode() + "_" + (ParentList == null ? "SITE" : ParentList.ID.ToString()) + "_" + propertyName;
        }

        /// <summary>
        /// Replace the buggy base implementation of SetCustomProperty
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        new public void SetCustomProperty(string propertyName, object propertyValue)
        {
            if (IsNew)
            {
                // field is being added - need to put property in cache
                CustomPropertiesForNewFields[GetCacheKey(propertyName)] = propertyValue + "";
            }

            CustomProperties[propertyName] = propertyValue + "";
        }

        /// <summary>
        /// Replace the buggy base implementation of GetCustomProperty
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        new public object GetCustomProperty(string propertyName)
        {
            if (!IsNew && CustomPropertiesForNewFields.ContainsKey(GetCacheKey(propertyName)))
            {
                string s = CustomPropertiesForNewFields[GetCacheKey(propertyName)];
                CustomPropertiesForNewFields.Remove(GetCacheKey(propertyName));
                CustomProperties[propertyName] = s;
                return s;
            }
            else
            {
                return CustomProperties[propertyName];
            }
        }

        /// <summary>
        /// Called when a field is created. Without this, update is not called and custom properties
        /// are not saved.
        /// </summary>
        /// <param name="op"></param>
        public override void OnAdded(SPAddFieldOptions op)
        {
            base.OnAdded(op);
            Update();
        }

        #endregion


        public override BaseFieldControl FieldRenderingControl
        {
            get
            {
                BaseFieldControl fieldControl = new $rootname$Control(this);

                fieldControl.FieldName = InternalName;

                return fieldControl;
            }
        }

        public override void Update()
        {
            SaveProperties();
            base.Update();
        }

        public string MyCustomProperty
        {
            get { return this.GetCustomProperty("MyCustomProperty") + ""; }
            set { this.SetCustomProperty("MyCustomProperty", value); }
        }


 
    }

}
