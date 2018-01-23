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
    public class UninstallWSP : CommandBase
    {
        #region constructor & descructor

        public UninstallWSP(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static UninstallWSP Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            UninstallWSP instance = new UninstallWSP(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "UNISTALLWSP",
                "&Uninstall",
                "Retracts and uninstalls the solution package from SharePoint",
                "Global::Ctrl+Shift+Alt+B,u", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            // Check if the wsp if newer than dll's, CS and xml files.
            WSPBuilderHandle wspTool = new WSPBuilderHandle(this.DTEInstance);
            ProjectPaths projectPaths = new ProjectPaths(this.DTEInstance.SelectedProject);

            wspTool.Uninstall(projectPaths.FullPath);
        }

        #endregion

    }
}