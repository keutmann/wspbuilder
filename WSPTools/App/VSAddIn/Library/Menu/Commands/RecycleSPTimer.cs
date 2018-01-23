#region namespace references
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.CommandBars;
using EnvDTE;
using EnvDTE80;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using WSPTools.BaseLibrary.SystemServices;

#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library.Commands
{
    public class RecycleSPTimer : CommandBase
    {
        #region constructor & descructor

        public RecycleSPTimer(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods

        public static RecycleSPTimer Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            RecycleSPTimer instance = new RecycleSPTimer(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "RECYCLESPTIMERV3",
                "&Recycle Services Timer",
                "Recycles the Windows SharePoint Services Timer",
                "Global::Ctrl+Shift+Alt+B,t", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            this.DTEInstance.BuildWindow.Clear();
            this.DTEInstance.BuildWindow.Activate();

            WindowsServices.Restart(WindowsServices.Current.SPTimerName);

            this.DTEInstance.WriteBuildAndStatusBar("Done recycling Windows SharePoint Services Timer!");
        }

        #endregion

    }
}