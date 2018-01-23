#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Windows.Forms;
using WSPTools.VisualStudio.VSAddIn.Library;
using System.Diagnostics;
#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    [ComImport,
    Guid("6D5140C1-7436-11CE-8034-00AA006009FA"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleServiceProvider
    {
        [PreserveSig]
        int QueryService(
            [In]ref Guid guidService,
            [In]ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out System.Object obj);
    }

    public class VSMenuHandler
    {
        #region constants

        private string MY_COMMAND_NAME = "MY_COMMAND";

        #endregion

        #region fields

        private _DTE _applicationObject;
        private AddIn _addInInstance;
        private Dictionary<string, CommandHandle> _commandList = new Dictionary<string, CommandHandle>();
        private int _position = 0;

        private List<CommandBarPopup> _customBarPopups = new List<CommandBarPopup>();
        private List<CommandBarControl> _commandControls = new List<CommandBarControl>();

        #endregion

        #region public properties

        public _DTE ApplicationObject
        {
            get { return _applicationObject; }
            set { _applicationObject = value; }
        }

        public AddIn AddInInstance
        {
            get { return _addInInstance; }
            set { _addInInstance = value; }
        }


        public Dictionary<string, CommandHandle> CommandList
        {
            get { return _commandList; }
            set { _commandList = value; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public string CommandName
        {
            get { return this.AddInInstance.Name + "." + MY_COMMAND_NAME; }
        }

        public List<CommandBarPopup> CustomBarPopups
        {
            get { return _customBarPopups; }
            set { _customBarPopups = value; }
        }

        public List<CommandBarControl> CommandControls
        {
            get { return _commandControls; }
            set { _commandControls = value; }
        }

        #endregion

        #region constructor & descructor

        public VSMenuHandler(_DTE applicationObject, AddIn addInInstance)
        {
            this.ApplicationObject = applicationObject;
            this.AddInInstance = addInInstance;
        }

        #endregion

        #region public methods

        
        public CommandBarPopup CreateCommandBarPopup(string name, string title, bool beginGroup, CommandBar parentCommandBar)
        {
            return this.CreateCommandBarPopup(name, title, beginGroup, ++Position, parentCommandBar);
        }

        public CommandBarPopup CreateCommandBarPopup(string name, string title, bool beginGroup, int position, CommandBar parentCommandBar)
        {
            object nullPar = Type.Missing;

            CommandBarPopup result = GetCommandBarPopup(parentCommandBar, title);
            if (result == null)
            {
                result = (CommandBarPopup)parentCommandBar.Controls.Add(
                    MsoControlType.msoControlPopup,
                    nullPar,
                    nullPar,
                    position,
                    true);

                result.CommandBar.Name = name;
                result.Caption = title;
                result.BeginGroup = beginGroup;
                result.Visible = true;

            }
            else
            {
                result.Visible = true;
            }

            CustomBarPopups.Add(result);

            return result;
        }

        public void DeleteCustomBarPopups()
        {
            Debug.WriteLine("Deleting Popups: " + this.CustomBarPopups.Count);

            foreach (CommandBarPopup bar in this.CustomBarPopups)
            {
                Debug.WriteLine("Deleting Popup: " + bar.CommandBar.Name);
                bar.Delete(true);
            }
        }

        public void DeleteAllCommandControls()
        {

            Debug.WriteLine("Deleting Controls: " + this.CommandControls.Count);

            foreach (CommandBarControl control in this.CommandControls)
            {
                Debug.WriteLine("Deleting Control: " + control.Caption);
                control.Delete(true);
            }
        }

        public Command CreateCommand(string name, string title, string description, string HotKey)
        {
            return CreateCommand(name, title, description, HotKey, null, null);
        }

        public Command CreateCommand(string name, string title, string description, string HotKey, ExecuteDelegate executeMethod)
        {
            return CreateCommand(name, title, description, HotKey, executeMethod, null);
        }

        public Command CreateCommand(string name, string title, string description, string HotKey, ExecuteDelegate executeMethod, StatusDelegate statusMethod)
        {
            object[] contextGUIDS = new object[] { };
            Command result = null;

            string fullname = GetFullName(name);
            result = GetCommand(fullname);

            
            if (result == null)
            {
                Commands2 commands = (Commands2)ApplicationObject.Commands;
                result = commands.AddNamedCommand2(
                    AddInInstance,
                    name,
                    title,
                    description,
                    true,
                    0,
                    ref contextGUIDS,
                    (int)vsCommandStatus.vsCommandStatusSupported +
                    (int)vsCommandStatus.vsCommandStatusEnabled,
                    (int)vsCommandStyle.vsCommandStylePictAndText,
                    vsCommandControlType.vsCommandControlTypeButton);

                // *** If a hotkey was provided try to set it
                //result.Bindings = bindings;
                if (HotKey != null && HotKey != "")
                {
                    object[] bindings = (object[])result.Bindings;
                    if (bindings != null)
                    {
                        bindings = new object[1];
                        bindings[0] = (object)HotKey;
                        try
                        {
                            result.Bindings = (object)bindings;
                        }
                        catch
                        {
                            // Do nothing
                        }
                    }
                }
            }

            if (!CommandList.ContainsKey(fullname))
            {
                CommandHandle handle = new CommandHandle();
                handle.CommandObject = result;
                handle.ExecuteMethod = executeMethod;
                handle.StatusMethod = statusMethod;

                CommandList.Add(fullname, handle);
            }

            return result;
        }


        public void AddToCommandBars(Command command, bool beginGroup, CommandBar[] commandBars)
        {
            foreach (CommandBar bar in commandBars)
            {
                AddToCommandBar(command, beginGroup, bar);
            }
        }

        public void AddToCommandBar(Command command, bool beginGroup, CommandBar commandBar)
        {
            bool found = false;
            foreach (CommandBarControl control in commandBar.Controls)
            {
                if (control is Command)
                {
                    Command currentCommand = control as Command;
                    if (currentCommand.Name == command.Name)
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Debug.WriteLine(String.Format("Adding command {0} to Bar {1}", command.Name, commandBar.Name));

                CommandBarControl control = (CommandBarControl)command.AddControl(commandBar, commandBar.Controls.Count + 1);
                //command.AddControl(commandBar, ++Position);
                control.BeginGroup = beginGroup;

                CommandControls.Add(control);
            }
        }

        //private bool CommandExistInBar(Command command, CommandBar bar)
        //{
        //    bool result = false;
        //    foreach (CommandBarControl barControl in bar.Controls)
        //    {

        //        if (barControl is CommandBarButton)
        //        {
        //            CommandBarButton button = (CommandBarButton)barControl;
        //            button.get_accName
        //            Command tempCmd = (Command)barControl.Control;
        //            if (tempCmd.Name.Equals(command.Name, StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                result = true;
        //                break;
        //            }
        //        }
        //    }
        //    return result;
        //}


        public void RemoveCommands(CommandBar projectBar, string nameSpace)
        {
            List<Command> commandsToDelete = new List<Command>();
            List<CommandBarControl> commandBarsToDelete = new List<CommandBarControl>();

            foreach (Command command in ApplicationObject.Commands)
            {

                if (command.Name.StartsWith(nameSpace, StringComparison.InvariantCulture))
                {
                    commandsToDelete.Add(command);
                }
            }
            foreach (Command command in commandsToDelete)
            {
                command.Delete();
            }

            //CommandBar menuBar = ((CommandBars)ApplicationObject.CommandBars)["MenuBar"];
            foreach (CommandBarControl control in projectBar.Controls)
            {
                if (control is CommandBarPopup)
                {
                    CommandBarPopup popup = (CommandBarPopup)control;
                    if (popup.CommandBar.Name.StartsWith("WSPBuilder", StringComparison.InvariantCultureIgnoreCase))
                    {
                        commandBarsToDelete.Add(popup);
                    }
                }
            }
            foreach (CommandBarControl barControl in commandBarsToDelete)
            {
                barControl.Delete(null);
            }
        }

        public CommandBar GetCommandBar(int id)
        {
            CommandBar result = null;
            CommandBars bars = (CommandBars)this.ApplicationObject.CommandBars;

            foreach (CommandBar bar in bars)
            {
                if (bar.Id == id)
                {
                    result = bar;
                    break;
                }
            }
            return result;
        }


        public CommandBar GetCommandBar(string commandBarName)
        {
            CommandBar result = null;
            CommandBars bars = (CommandBars)this.ApplicationObject.CommandBars;
            string localName = this.GetLocalizedName(commandBarName);

            foreach (CommandBar bar in bars)
            {
                Debug.WriteLine("Name: " + bar.NameLocal + " : ID: " + bar.Id);
                if (bar.NameLocal.Equals(localName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = bar;
                    break;
                }
            }
            return result;        
        }

        public CommandBarPopup GetCommandBarPopup(string commandGroup, string name)
        {
            string localGroupName = this.GetLocalizedName(commandGroup);
            CommandBar menuBar = ((CommandBars)ApplicationObject.CommandBars)[localGroupName];
            return this.GetCommandBarPopup(menuBar, name);
        }

        public CommandBarPopup GetCommandBarPopup(CommandBar parentCommandBar, string name)
        {
            CommandBarPopup result = null;

            string localName = this.GetLocalizedName(name);
            foreach (CommandBarControl control in parentCommandBar.Controls)
            {
                if (control is CommandBarPopup)
                {
                    CommandBarPopup popup = (CommandBarPopup)control;
                    if (!String.IsNullOrEmpty(popup.CommandBar.Name))
                    {
                        Debug.WriteLine("CommandBarPopup.Name: " + popup.CommandBar.Name);
                        string commandBarName = popup.CommandBar.Name.Replace("&", "");
                        if (popup.CommandBar.Name.Equals(localName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            result = popup;
                            Debug.WriteLine("CommandBarPopup found: " + result.CommandBar.Name);

                            break;
                        }
                    }
                }
            }

            return result;
        }



        public string GetLocalizedName(string commandbarName)
        {
            string nameSpace = Assembly.GetExecutingAssembly().FullName;
            string name = string.Empty;
            try
            {
                //  If you would like to move the command to a different menu, change the word "Tools" to the 
                //  English version of the menu. This code will take the culture, append on the name of the menu
                //  then add the command to that menu. You can find a list of all the top-level menus in the file
                //  CommandBar.resx.
                ResourceManager resourceManager = new ResourceManager(nameSpace + ".CommandBar", Assembly.GetExecutingAssembly());
                CultureInfo cultureInfo = new System.Globalization.CultureInfo(this.ApplicationObject.LocaleID);
                string resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, name);
                name = resourceManager.GetString(resourceName);
            }
            catch
            {
                //We tried to find a localized version of the word Tools, but one was not found.
                //  Default to the en-US word, which may work for the current culture.
                name = commandbarName;
            }
            return name;
        }


        public void Execute(string commandName)
        {
            if (this.CommandList.ContainsKey(commandName))
            {
                ExecuteDelegate execMethod = this.CommandList[commandName].ExecuteMethod;
                if (execMethod != null)
                {
                    execMethod.Invoke();
                }
            }
        }

        public vsCommandStatus QueryStatus(string commandName)
        {
            vsCommandStatus status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;
            if (this.CommandList.ContainsKey(commandName))
            {
                CommandHandle handle = this.CommandList[commandName];

                if (handle.StatusMethod != null)
                {
                    status = handle.StatusMethod.Invoke();
                }
                else
                {
                    bool found = true;

                    if (found)
                    {
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    }
                }
            }

            return status;
        }


        #endregion

        #region private methods

        private CommandBarPopup GetCommandBarPopup(string name, CommandBar parentCommandBar)
        {
            CommandBarPopup result = null;
            string localName = this.GetLocalizedName(name);

            foreach (CommandBarControl control in parentCommandBar.Controls)
            {
                if (control is CommandBarPopup)
                {
                    CommandBarPopup popup = (CommandBarPopup)control;
                    Debug.WriteLine("Name: " + popup.CommandBar.Name);



                    if (localName.Equals(popup.CommandBar.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = popup;
                        break;
                    }
                }
            }
            return result;
        }

        //private CommandBar GetCommandBar(string name)
        //{
        //    CommandBar menuBarCommandBar = ((CommandBars)ApplicationObject.CommandBars)["MenuBar"];
        //    CommandBar result = null;
        //    string localName = this.GetLocalizedName(name);

        //    foreach (CommandBarControl control in menuBarCommandBar.Controls)
        //    {
        //        if (control is CommandBar)
        //        {
        //            CommandBar bar = (CommandBar)control;
        //            if (localName.Equals(bar.NameLocal, StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                result = bar;
        //                break;
        //            }
        //        }
        //    }
        //    return result;
        //}

        public CommandBar GetCommandBar(MenuItemDefinition menuDefinition)
        {
            CommandBar bar = null;
            try
            {
                bar = GetCommandBar(menuDefinition.GuidID, menuDefinition.ID);
            }
            catch 
            {
                try
                {
                    CommandBar menuBarCommandBar = ((CommandBars)ApplicationObject.CommandBars)["MenuBar"];

                    CommandBarPopup toolsPopup = GetCommandBarPopup(menuDefinition.Name, menuBarCommandBar);
                    bar = toolsPopup.CommandBar;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Unable to get commandbar : " + menuDefinition.Name);
#if DEBUG
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
#endif
                    throw;
                }

            }

            return bar;
        }

        /// <summary>
        /// http://blogs.msdn.com/dr._ex/archive/2007/04/17/using-ivsproffercommands-to-retrieve-a-visual-studio-commandbar.aspx
        /// </summary>
        /// <param name="guidCmdGroup"></param>
        /// <param name="menuID"></param>
        /// <returns></returns>
        public CommandBar GetCommandBar(Guid guidCmdGroup, uint menuID)
        {
            // Make sure the CommandBars collection is properly initialized, before we attempt to
            // use the IVsProfferCommands service.
            CommandBar menuBarCommandBar = ((CommandBars)ApplicationObject.CommandBars)["MenuBar"];

            // Retrieve IVsProfferComands via DTE's IOleServiceProvider interface
            IOleServiceProvider sp = (IOleServiceProvider)ApplicationObject;
            Guid guidSvc = typeof(IVsProfferCommands).GUID;
            Object objService;
            sp.QueryService(ref guidSvc, ref guidSvc, out objService);
            IVsProfferCommands vsProfferCmds = (IVsProfferCommands)objService;
            CommandBar result = vsProfferCmds.FindCommandBar(IntPtr.Zero, ref guidCmdGroup, menuID) as CommandBar;
            return result;
        }





        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private Command GetCommand(string name)
        {
            Command result = null;

            foreach (Command command in ApplicationObject.Commands)
            {

                if (name.Equals(command.Name))
                {
                    result = command;
                }
            }
            return result;
        }

        private string GetFullName(string name)
        {
            return AddInInstance.ProgID + "." + name;
        }

        #endregion

    }
}