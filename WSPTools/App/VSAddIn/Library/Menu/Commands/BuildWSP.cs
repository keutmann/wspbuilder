#region namespace references
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.CommandBars;
using EnvDTE;
using EnvDTE80;
using System.IO;
using Microsoft.Win32;

#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library.Commands
{
    public class BuildWSP : CommandBase
    {
        #region constants
        #endregion

        #region fields
        #endregion

        #region public properties
        #endregion

        #region constructor & descructor

        public BuildWSP(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static BuildWSP Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            BuildWSP instance = new BuildWSP(dteHandler);
            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "BUILDPROJECTWSP", 
                "&Build WSP", 
                "Build a SharePoint solution from current project", 
                "Global::Ctrl+Shift+Alt+B,b", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }

        protected override void Execute()
        {
            WSPFileHandle wspHandle = new WSPFileHandle(this.DTEInstance);
            
            // this.DTEInstance.Application.Solution.SolutionBuild.Build(true);

            ProjectPaths projectPaths = new ProjectPaths(this.DTEInstance.SelectedProject);
            WSPBuilderHandle wspTool = new WSPBuilderHandle(this.DTEInstance);
            wspTool.RunWSPBuilder(wspHandle.SelectedProject.FullPath, "-BuildWSP true -projectassembly \"" + projectPaths.OutputFilePath + "\"");
        }

        #endregion

    }
}