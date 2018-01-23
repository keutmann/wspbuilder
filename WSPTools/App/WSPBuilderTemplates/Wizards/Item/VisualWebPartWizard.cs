/* Program : WSPBuilderTemplates
 * Created by: Carsten Keutmann
 * Date : 2009
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using WSPTools.BaseLibrary.Win32;

namespace WSPBuilderTemplates
{

    public class VisualWebPartWizard : WebPartWizard
    {
        public VisualWebPartWizard()
        {

        }

        public override void AddParameterReplacements()
        {
            base.AddParameterReplacements();

            if (SharePointRegistry.IsSharePoint14)
            {
                this.Replacements[ItemPropertyValues.SharePoint2010Content] = @"<%@ Assembly Name=""Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %>";
            }
        }


        public override void AddReferences()
        {
            base.AddReferences();

            if (SharePointRegistry.IsSharePoint14)
            {
                this.AddReference("System.Web.Extensions");
            }
        }

    }
}
