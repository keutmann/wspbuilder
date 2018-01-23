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
    public class CreateDeploymentFolder : CommandBase
    {
        #region constructor & descructor

        public CreateDeploymentFolder(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static CreateDeploymentFolder Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            CreateDeploymentFolder instance = new CreateDeploymentFolder(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "CREATEDEPLOYMENTFOLDER",
                "Create Deployment &Folder",
                "Creates a folder that can be copied to deloy on other systems",
                "Global::Ctrl+Shift+Alt+B,f", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        /// <summary>
        /// Create a deployment folder
        /// Added Feb 2008 by Tom Clarkson
        /// </summary>
        protected override void Execute()
        {
            WSPFileHandle wspHandle = new WSPFileHandle(this.DTEInstance);
            WSPBuilderHandle wspTool = new WSPBuilderHandle(this.DTEInstance);
            wspTool.RunWSPBuilder(wspHandle.SelectedProject.FullPath, "-BuildWSP true -CreateDeploymentFolder all ");
        }

        #endregion

    }
}