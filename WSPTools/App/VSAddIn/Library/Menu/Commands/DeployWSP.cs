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
    public class DeployWSP : CommandBase
    {
        #region constructor & descructor

        public DeployWSP(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static DeployWSP Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            DeployWSP instance = new DeployWSP(dteHandler);
            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "DEPLOYWSP", 
                "&Deploy", 
                "Installs and deploys the solution package to SharePoint globally", 
                "Global::Ctrl+Shift+Alt+B,d", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            // Check if the wsp if newer than dll's, CS and xml files.
            WSPFileHandle wspHandle = new WSPFileHandle(this.DTEInstance);
            WSPBuilderHandle wspTool = new WSPBuilderHandle(this.DTEInstance);

            DialogResult result = DialogResult.No;
            if (wspHandle.ProjectFilesHaveChanged())
            {
                result = MessageBox.Show(Strings.NEEDTOREBUILDSOLUTION,
                                         Strings.REBUILD,
                                         MessageBoxButtons.YesNoCancel);
            }
            if (result == DialogResult.Yes)
            {
                this.DTEInstance.Application.Solution.SolutionBuild.Build(true);
                if (this.DTEInstance.Application.Solution.SolutionBuild.LastBuildInfo > 0)
                {
                    MessageBox.Show("Build Failed. Unable to deploy");
                }
                else
                {
                    wspTool.RunWSPBuilder(wspHandle.SelectedProject.FullPath, "-BuildWSP true -deploy true");
                }
            }
            else
            {
                if (result == DialogResult.No)
                {
                    wspTool.Deploy(wspHandle.SelectedProject.FullPath);
                }
            }
        }

        protected override vsCommandStatus Status()
        {
            vsCommandStatus status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;

            if (this.DTEInstance.SelectedProject != null)
            {
                WSPFileHandle wspHandle = new WSPFileHandle(this.DTEInstance);
                if (wspHandle.WspFilename != null)
                {
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                }
            }

            return status;
        }


        #endregion

    }
}