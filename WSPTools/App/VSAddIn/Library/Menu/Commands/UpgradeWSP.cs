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
    public class UpgradeWSP : CommandBase
    {
        #region constructor & descructor

        public UpgradeWSP(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static UpgradeWSP Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            UpgradeWSP instance = new UpgradeWSP(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "UPGRADEWSP",
                "Up&grade",
                "Upgrads the solution package",
                "Global::Ctrl+Shift+Alt+B,p", 
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
                result = MessageBox.Show("One or more files in the project have changed!\r\n\r\n" +
                                         "Do you want to rebuild the project and create a\r\n" +
                                         "new solution package (WSP) before upgrading?",
                                         "Rebuild!",
                                         MessageBoxButtons.YesNoCancel);
            }
            if (result == DialogResult.Yes)
            {
                this.DTEInstance.Application.Solution.SolutionBuild.Build(true);
                wspTool.RunWSPBuilder(wspHandle.SelectedProject.FullPath, "-BuildWSP true -Upgrade " + wspHandle.WspFileInfo.Name);
            }
            else
            {
                if (result == DialogResult.No)
                {
                    wspTool.Upgrade(wspHandle.SelectedProject.FullPath);
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