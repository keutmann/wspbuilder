#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class Strings
    {
        #region constants

        public const string NEEDTOREBUILDSOLUTION = "One or more files in the project have changed!\r\n\r\n" +
                                         "Do you want to rebuild the project and create a\r\n" +
                                         "new solution package (WSP) before deployment?";
        public const string REBUILD = "Rebuild";
        #endregion

    }
}