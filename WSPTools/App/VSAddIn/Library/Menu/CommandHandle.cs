#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    public class CommandHandle
    {
        #region fields

        private Command _commandObject = null;

        private ExecuteDelegate _executeMethod = null;
        private StatusDelegate _statusMethod = null;

        #endregion

        #region public properties

        public Command CommandObject
        {
            get { return _commandObject; }
            set { _commandObject = value; }
        }

        public ExecuteDelegate ExecuteMethod
        {
            get { return _executeMethod; }
            set { _executeMethod = value; }
        }

        public StatusDelegate StatusMethod
        {
            get { return _statusMethod; }
            set { _statusMethod = value; }
        }

        #endregion

        #region constructor & descructor

        public CommandHandle()
        {
        }

        #endregion

        #region protected/internal methods
        #endregion

        #region public methods
        #endregion

        #region private methods
        #endregion
    }
}