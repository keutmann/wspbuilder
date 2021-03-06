/* Program : WSPBuilderVSAddIn
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * Changes
 * ---------------------------------------------------------------------------- 
 * 20080626 Keutmann
 * Command "Copy to GAC" Added.
 * Code refactored.
 * 
 * 20080614 Keutmann
 * Total refactoring of the Addin
 * 
 * 20080216 Keutmann
 * Added the GetCommandBarPopup method to support localized versions of Visual Studio
 * Added code to support Showing and hidding of the WSPBuilder menu
 * Log class added for debug 
 *
 * 20080221 TQC
 * Fixed Attach to Process
 * Added check to DeployWSP to stop solution deployment and display a message if the build failed
 * 
 */
using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.Win32;
using WSPTools.VisualStudio.VSAddIn.Library;
using System.Resources;
using System.Runtime.InteropServices;
using WSPTools.VisualStudio.VSAddIn.Library.Commands;
using Microsoft.VisualStudio;


namespace WSPTools.VisualStudio.VSAddIn
{
    [GuidAttribute("906eb397-58c7-42c4-b874-8f47f271537c"), ProgId("WSPTools.VisualStudio.VSAddIn")]
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        #region Members

        private WSPBuilderHandle _wspTool = null;

        private DTEHandler _DTEInstance = null;
        private CommandBar[] _allbars = null;
        private CommandBar[] ProjectBars = null;

        #endregion

        #region Properties


        public WSPBuilderHandle WSPTool
        {
            get
            {
                if (_wspTool == null)
                {
                    _wspTool = new WSPBuilderHandle(this.DTEInstance);
                }
                return _wspTool;
            }
            set { _wspTool = value; }
        }


        public DTEHandler DTEInstance
        {
            get { return _DTEInstance; }
            set { _DTEInstance = value; }
        }

        public CommandBar[] Allbars
        {
            get { return _allbars; }
            set { _allbars = value; }
        }

        #endregion

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }


        #region Events


        /// <summary>
        /// Adds the Commandbar to the context menu.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="connectMode"></param>
        /// <param name="addInInst"></param>
        /// <param name="custom"></param>
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            try
            {
                this.DTEInstance = new DTEHandler((DTE2)application, (AddIn)addInInst);
                switch (connectMode)
                {
                    case ext_ConnectMode.ext_cm_AfterStartup:
                        CreateCommands();
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "WSPBuilder AddIn Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adds the Commands and Commandbars to the DTE.
        /// </summary>
        /// <param name="application">DTE object</param>
        /// <param name="addInInst">AddIn Instance</param>
        //private void OnStartupComplete(DTE2 application, AddIn addInInst)
        private void CreateCommands()
        {
#if DEBUG
            Log.Write("Now Logging!");
#endif

            CommandBar toolsMenuBar = this.DTEInstance.Menu.GetCommandBar(VSMenuConstants.MainToolsMenu);
            CommandBar projectContextBar = this.DTEInstance.Menu.GetCommandBar(VSMenuConstants.ProjectContextMenu);
            CommandBar folderContextBar = this.DTEInstance.Menu.GetCommandBar(VSMenuConstants.FolderContextMenu);
            CommandBar itemContextBar = this.DTEInstance.Menu.GetCommandBar(VSMenuConstants.ItemContextMenu);

            //CleanUpPopup(toolsMenuBar, "WSPBuilder");
            //CleanUpPopup(projectContextBar, "WSPBuilder");

            // Create or get the commandbar popup
            CommandBarPopup toolsWSPBuilderPopup = this.DTEInstance.Menu.CreateCommandBarPopup("toolsWSPBuilderPopup", "WSPBuilder", true, 1, toolsMenuBar);
            CommandBarPopup projectWSPBuilderPopup = this.DTEInstance.Menu.CreateCommandBarPopup("projectWSPBuilderPopup", "WSPBuilder", true, 1, projectContextBar);

            Allbars = new CommandBar[] { 
                projectWSPBuilderPopup.CommandBar, 
                toolsWSPBuilderPopup.CommandBar, 
                itemContextBar, 
                folderContextBar };

            ProjectBars = new CommandBar[] { 
                projectWSPBuilderPopup.CommandBar, 
                toolsWSPBuilderPopup.CommandBar };


            BuildWSP.Create(this.DTEInstance, false, ProjectBars);
            DeployWSP.Create(this.DTEInstance, false, ProjectBars);
            UpgradeWSP.Create(this.DTEInstance, false, ProjectBars);
            UninstallWSP.Create(this.DTEInstance, false, ProjectBars);

            // Add this command to the Document Context Menu if its a 12 hive file!
            CopyToSharePointRoot.Create(this.DTEInstance, true, Allbars); // Add also to the Item Context Menu
            CopyToGAC.Create(this.DTEInstance, false, ProjectBars);

            RecycleAppPools.Create(this.DTEInstance, true, ProjectBars);
            RecycleSPTimer.Create(this.DTEInstance, false, ProjectBars);
            CreateDeploymentFolder.Create(this.DTEInstance, false, ProjectBars);
            AttachToWorkerProcesses.Create(this.DTEInstance, false, ProjectBars);
        }



        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            try
            {
                if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
                {
                    status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;
                    if (this.DTEInstance.RunningCommand)
                    {
                        status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;
                    }
                    else
                    {
                        status = this.DTEInstance.Menu.QueryStatus(commandName);
                    }
                }
            }
            catch (Exception ex)
            {
                // Sometimes the Query status throws a lot of exceptions. Therefore do not show this, just trace it.
                Trace.WriteLine(ex.ToString(), "WSPBuilder Extensions");
                //Log.Error(ex);
            }
        }


        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            try
            {
                if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
                {
                    if (this.DTEInstance.Application != null)
                    {
                        this.DTEInstance.Menu.Execute(commandName);
                        handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "WSPBuilder AddIn Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                handled = true;

                Log.Error(ex);
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            try
            {
                Debug.WriteLine("OnDisconnection called!");

                if (disconnectMode == ext_DisconnectMode.ext_dm_HostShutdown ||
                    disconnectMode == ext_DisconnectMode.ext_dm_UserClosed)
                {
                    Debug.WriteLine("ext_DisconnectMode: " + disconnectMode);

                    // Important: First clear out every command control
                    //this.DTEInstance.Menu.DeleteAllCommandControls();
                    // Then clear out every command bar
                    //this.DTEInstance.Menu.DeleteCustomBarPopups();
                    foreach (CommandBarPopup commandBarPopup in this.DTEInstance.Menu.CustomBarPopups)
                    {
                        CleanUpPopup(commandBarPopup.Parent, commandBarPopup.CommandBar.Name);
                    }

                    CleanUpCommands();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "WSPBuilder AddIn Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex);
            }
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
             
            //Log.Write(this, "OnAddInsUpdate()");
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
            CreateCommands();
            //Log.Write(this, "OnStartupComplete()");
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
            //Log.Write(this, "OnBeginShutdown() - Connect variables is set " + (wspBuilderMenuControl != null));
        }

        private void CleanUpPopup(CommandBar commandBar, string name)
        {
            Debug.WriteLine("Commandbars to delete: " + commandBar.Controls.Count);
            List<CommandBarPopup> popups = new List<CommandBarPopup>();
            foreach (CommandBarControl control in commandBar.Controls)
            {
                if (control is CommandBarPopup)
                {
                    popups.Add(control as CommandBarPopup);
                }
            }

            foreach (CommandBarPopup commandbarPopup in popups)
            {
                if (commandbarPopup.CommandBar.Name == name)
                {
                    CleanUpControls(commandbarPopup);
                    Debug.WriteLine("Commandbar delete: " + commandbarPopup.CommandBar.Name + " from Bar " + commandBar.Name);
                    commandbarPopup.Delete(false);
                }
            }
        }

        private void CleanUpControls(CommandBarPopup commandBar)
        {
            Debug.WriteLine("Controls delete on command bar: " + commandBar.Caption);
            List<CommandBarControl> barControls = new List<CommandBarControl>();
            foreach (CommandBarControl barControl in commandBar.Controls)
            {
                barControls.Add(barControl);
            }

            foreach (CommandBarControl barControl in barControls)
            {
                Debug.WriteLine("Control Deleted: " + barControl.Caption);
                barControl.Delete(false);
            }


            //foreach (CommandBarControl createdControl in this.DTEInstance.Menu.CommandControls)
            //{
            //    createdControl.Delete(true);
            //    Debug.WriteLine("Control Deleted: " + createdControl.Caption);
            //    //foreach (CommandBarControl barControl in barControls)
            //    //{
                    
            //    //    if (barControl.Caption == createdControl.Caption)
            //    //    {
            //    //        try
            //    //        {
            //    //        }
            //    //        catch (Exception)
            //    //        {
                            
            //    //            throw;
            //    //        }
            //    //        barControl.Delete(true);
            //    //        Debug.WriteLine("Control Deleted: " + barControl.Caption);
            //    //    }
            //    //}
            //}
        }

        private void CleanUpCommands()
        {
            Debug.WriteLine("Commands delete!");
            foreach (CommandHandle commandHandle in this.DTEInstance.Menu.CommandList.Values)
            {
                Debug.WriteLine("Command delete: " + commandHandle.CommandObject.Name);
                commandHandle.CommandObject.Delete();
            }
        }

        #endregion
    }
}

