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
    public class AttachToWorkerProcesses : CommandBase
    {
        #region constructor & descructor

        public AttachToWorkerProcesses(DTEHandler dteHandler)
            : base(dteHandler)
        {
        }

        #endregion

        #region public methods


        public static AttachToWorkerProcesses Create(DTEHandler dteHandler, bool beginGroup, CommandBar[] commandbars)
        {
            AttachToWorkerProcesses instance = new AttachToWorkerProcesses(dteHandler);

            instance.CommandInstance = dteHandler.Menu.CreateCommand(
                "ATTACHTOWORKERPROCESSES",
                "&Attach to IIS Worker Processes",
                "Attaches the debugger to all instances of w3wp.exe",
                "Global::Ctrl+Shift+Alt+B,a", 
                new ExecuteDelegate(instance.Execute),
                new StatusDelegate(instance.Status));

            dteHandler.Menu.AddToCommandBars(instance.CommandInstance, beginGroup, commandbars);

            return instance;
        }



        /// <summary>
        /// Attach the debugger to all running instances of w3wp.exe
        /// Added Feb 2008 by Tom Clarkson
        /// Based on http://weblogs.asp.net/koenv/archive/2008/02/14/quick-attach-and-detach-debugger.aspx
        /// </summary>        
        protected override void Execute()
        {
            Debugger2 debugger = (Debugger2)DTEInstance.Application.Debugger;

            Processes processes = debugger.LocalProcesses;
            foreach (Process2 process in processes)
            {
                if (process.Name.EndsWith("w3wp.exe"))
                {
                    process.Attach();
                }
                if (process.Name.EndsWith("W3SVC.exe"))
                {
                    process.Attach();
                }
            }
        }

        #endregion

    }
}