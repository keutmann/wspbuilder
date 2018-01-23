#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    public delegate void ExecuteDelegate();
    public delegate vsCommandStatus StatusDelegate();
}