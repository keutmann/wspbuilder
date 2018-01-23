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

#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library.Commands
{
    public class RecycleAppPools : CommandBase
    {
        #region constructor & descructor

        public RecycleAppPools(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods

        public static RecycleAppPools Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            RecycleAppPools instance = new RecycleAppPools(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "RECYCLEAPPPOOLS",
                "&Recycle AppPools",
                "Recycles the applicationpools (IIS)",
                "Global::Ctrl+Shift+Alt+B,r", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            this.DTEInstance.BuildWindow.Clear();
            this.DTEInstance.BuildWindow.Activate();

            AppPoolHandle appPools = new AppPoolHandle(this.DTEInstance);
            appPools.RecycleAppPools();

            this.DTEInstance.WriteBuildAndStatusBar("Done recycling Application pools!");
        }

        #endregion

    }
}