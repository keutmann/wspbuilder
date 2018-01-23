#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio.CommandBars;
#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library.Commands
{
    public class CommandBase
    {
        #region constants
        #endregion

        #region fields

        private DTEHandler _DTEInstance = null;
        private Command _commandInstance = null;
        private CommandBarControl _barControl = null;

        #endregion

        #region public properties

        public DTEHandler DTEInstance
        {
            get { return _DTEInstance; }
            set { _DTEInstance = value; }
        }

        public Command CommandInstance
        {
            get { return _commandInstance; }
            set { _commandInstance = value; }
        }

        public CommandBarControl CommandControl
        {
            get { return _barControl; }
            set { _barControl = value; }
        }

        #endregion


        #region constructor & descructor

        public CommandBase(DTEHandler dteHandler)
        {
            this.DTEInstance = dteHandler;
        }

        #endregion

        #region private methods
        #endregion

        #region protected/internal methods
        #endregion

        #region public methods

        //public static void AddToCommandBars(CommandBase instance, bool beginGroup, CommandBar[] commandbars)
        //{

        //    foreach (CommandBar bar in commandbars)
        //    {
        //        instance.CommandControl = (CommandBarControl)instance.CommandInstance.AddControl(bar, bar.Controls.Count + 1);
        //        instance.CommandControl.BeginGroup = beginGroup;
        //    }
        //}


        protected virtual void Execute()
        {
        }

        protected virtual vsCommandStatus Status()
        {
            vsCommandStatus result = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;
            if (this.DTEInstance.Application.Solution.IsOpen)
            {
                result = vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported;
            }
            return result;
        }


        #endregion


        #region event handlers
        #endregion
    }
}