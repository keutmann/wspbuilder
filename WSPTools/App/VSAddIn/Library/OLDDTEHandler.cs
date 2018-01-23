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
using WSPTools.VisualStudio.VSAddIn.Library;
#endregion

namespace WSPTools.VisualStudio.VSAddIn
{
    public class OLDDTEHandler
    {
        #region constants
        #endregion

        #region fields

        private static DTEHandler _application = null;

        private string _className = null;
        private string _namespace = null;

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        private OutputWindow _applicationOutputWindow = null;

        private int _position = 0;

        private Dictionary<string, CommandHandle> _commandList = new Dictionary<string, CommandHandle>();

        private OutputWindowPane _buildWindow = null;

        #endregion

        #region public properties

        public static DTEHandler Application
        {
            get
            {
                return _application;
            }
            set
            {
                _application = value;
            }
        }

        public UIHierarchyItem FirstSelectedItem
        {
            get
            {
                UIHierarchy UIH = ApplicationObject.ToolWindows.SolutionExplorer;
                return  (UIHierarchyItem)((System.Array)UIH.SelectedItems).GetValue(0);
            }
        }

        public Project SelectedProject
        {
            get
            {
                return FirstSelectedItem.Object as Project;
            }
        }

        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }

        public string Namespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        public DTE2 ApplicationObject
        {
            get { return _applicationObject; }
            set { _applicationObject = value; }
        }

        public AddIn AddInInstance
        {
            get { return _addInInstance; }
            set { _addInInstance = value; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }


        public Dictionary<string, CommandHandle> CommandList
        {
            get { return _commandList; }
            set { _commandList = value; }
        }




        public OutputWindow ApplicationOutputWindow
        {
            get 
            {
                if (_applicationOutputWindow == null)
                {
                    _applicationOutputWindow = ApplicationObject.ToolWindows.OutputWindow;
                }
                return _applicationOutputWindow; 
            }
            set { _applicationOutputWindow = value; }
        }


        public OutputWindowPane BuildWindow
        {
            get
            {
                if (_buildWindow == null)
                {
                    // Add-in code.
                    // Create a reference to the Output window.
                    // Create a tool window reference for the Output window
                    // and window pane.
                    _buildWindow = ApplicationOutputWindow.OutputWindowPanes.Item("Build");
                }
                return _buildWindow;
            }
            set { _buildWindow = value; }
        }


        #endregion

        #region constructor & descructor


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static OLDDTEHandler()
        {
        }

        private OLDDTEHandler(Type classType, DTE2 applicationObject, AddIn addInInstance)
        {
            this.ClassName = classType.Name;
            this.Namespace = classType.Namespace;
            this.ApplicationObject = applicationObject;
            this.AddInInstance = addInInstance;
        }

        #endregion

        #region event handlers
        #endregion

        #region public methods

        public CommandBarPopup CreateCommandBarPopup(string name, string title, CommandBar parentCommandBar)
        {
            return this.CreateCommandBarPopup(name, title, ++Position, parentCommandBar);
        }

        public CommandBarPopup CreateCommandBarPopup(string name, string title, int position, CommandBar parentCommandBar)
        {
            object nullPar = Type.Missing;

            CommandBarPopup result = null; // GetCommandBarPopup(name, parentCommandBar);
            if (result == null)
            {
                result = (CommandBarPopup)parentCommandBar.Controls.Add(
                    MsoControlType.msoControlPopup, 
                    nullPar, 
                    nullPar, 
                    position, 
                    nullPar);

                result.CommandBar.Name = name;
                result.Caption = title;
                Log.Write("CreateCommandBarPopup(" + name + ")");
            }
            return result;
        }


        public Command CreateCommand(string name, string title, string description, string HotKey, CommandBarPopup parentCommandBar)
        {
            return CreateCommand(name, title, description, HotKey, parentCommandBar, null, null);
        }

        public Command CreateCommand(string name, string title, string description, string HotKey, CommandBarPopup parentCommandBar, ExecuteDelegate executeMethod)
        {
            return CreateCommand(name, title, description, HotKey, parentCommandBar, executeMethod, null);
        }

        public Command CreateCommand(string name, string title, string description, string HotKey, CommandBarPopup parentCommandBar, ExecuteDelegate executeMethod, StatusDelegate statusMethod)
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

                result.AddControl(parentCommandBar.CommandBar, ++Position);

                Log.Write("CreateCommand(" + name + ")");
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


        public void Execute(string commandName)
        {
            foreach (KeyValuePair<string, CommandHandle> entry in CommandList)
            {
                if (commandName.Equals(entry.Key))
                {
                    // Execute the method for this command
                    entry.Value.ExecuteMethod.Invoke();
                }
            }
        }

        public vsCommandStatus QueryStatus(string commandName, WSPBuilderHandle WSPTool)
        {
            vsCommandStatus status = vsCommandStatus.vsCommandStatusNinched | vsCommandStatus.vsCommandStatusSupported;
            if (WSPTool.Running)
            {
                // The WSPTool is runnig no menu button available
                status |= vsCommandStatus.vsCommandStatusNinched;
            }
            else
            {
                if (CommandList.ContainsKey(commandName))
                {
                    CommandHandle handle = CommandList[commandName];

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
            }

            return status;
        }


        public void RemoveCommands(CommandBar projectBar)
        {
            List<Command> commandsToDelete = new List<Command>();
            List<CommandBarControl> commandBarsToDelete = new List<CommandBarControl>();

            foreach (Command command in ApplicationObject.Commands)
            {
                if (command.Name.StartsWith(this.Namespace, StringComparison.InvariantCulture))
                {
                    commandsToDelete.Add(command);
                    Log.Write("RemoveCommands(" + command.Name + ")");
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
                        Log.Write("RemoveCommandsPopup(" + popup.CommandBar.Name + ")");
                    }
                }
            }
            foreach (CommandBarControl barControl in commandBarsToDelete)
            {
                barControl.Delete(null);
            }
        }

       
        public CommandBarPopup GetCommandBarPopup(string name, string CommandGroup)
        {
            CommandBarPopup result = null;

            CommandBar menuBar = ((CommandBars)ApplicationObject.CommandBars)[CommandGroup];

            foreach (CommandBarControl control in menuBar.Controls)
            {
                if (control is CommandBarPopup)
                {
                    CommandBarPopup popup = (CommandBarPopup)control;
                    if(popup.CommandBar.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = popup;
                        break;
                    }
                }
            }

            return result;
        }


        public CommandBar GetSystemCommandBar(string name)
        {
            string toolsMenuName = string.Empty;
            try
            {
                //If you would like to move the command to a different menu, change the word "Tools" to the 
                //  English version of the menu. This code will take the culture, append on the name of the menu
                //  then add the command to that menu. You can find a list of all the top-level menus in the file
                //  CommandBar.resx.
                ResourceManager resourceManager = new ResourceManager(this.Namespace + ".CommandBar", Assembly.GetExecutingAssembly());
                CultureInfo cultureInfo = new System.Globalization.CultureInfo(this.ApplicationObject.LocaleID);
                string resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, name);
                toolsMenuName = resourceManager.GetString(resourceName);
            }
            catch
            {
                //We tried to find a localized version of the word Tools, but one was not found.
                //  Default to the en-US word, which may work for the current culture.
                toolsMenuName = name;
            }

            CommandBar bar = ((CommandBars)DTEHandler.Application.ApplicationObject.CommandBars)[toolsMenuName];

            return bar;

        }

        public bool IsWSPBuilderProject()
        {
            bool found = false;
            try
            {
                if (((System.Array)ApplicationObject.ToolWindows.SolutionExplorer.SelectedItems).Length > 0)
                {
                    // Detect if the currect project is a WSP project
                    foreach (ProjectItem item in SelectedProject.ProjectItems)
                    {
                        string name = item.Name;
                        if (name.Equals("12") ||
                            name.Equals("80") ||
                            name.Equals("GAC") ||
                            name.Equals("wspproject.xml", StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) {
                // If something went wrong, the folder wasn't found.
                Log.Write(ex.ToString());

            }

            return found;
        }


        #endregion

        #region Public Static Methods


        public static void CreateHandler(Type classType, DTE2 applicationObject, AddIn addInInstance)
        {
            if (Application == null)
            {
                Application = new DTEHandler(classType, applicationObject, addInInstance);
            }
        }

        public static void StatusBar(string message)
        {
            if (Application != null)
            {
                Application.ApplicationObject.StatusBar.Text = message;
            }
        }

        /// <summary>
        /// Clears the build window and then activates it
        /// </summary>
        public static void StartNewBuildWindow()
        {
            DTEHandler.Application.BuildWindow.Clear();
            DTEHandler.Application.BuildWindow.Activate();
            DTEHandler.Application.ApplicationOutputWindow.Parent.Activate();
        }

        public static void WriteBuildWindow(string text)
        {

            if(_application != null)
            {
                Application.BuildWindow.OutputString(text+"\r\n");
            }
        }

        #endregion

        #region private methods

        private CommandBarPopup GetCommandBarPopup(string name, CommandBarPopup parentCommandBar)
        {
            CommandBarPopup result = null;
            foreach (CommandBarControl control in parentCommandBar.Controls)
            {
                if (control is CommandBarPopup)
                {
                    CommandBarPopup popup = (CommandBarPopup)control;
                    if (name.Equals(popup.CommandBar.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = popup;
                        break;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// http://blogs.msdn.com/dr._ex/archive/2007/04/17/using-ivsproffercommands-to-retrieve-a-visual-studio-commandbar.aspx
        /// </summary>
        /// <param name="guidCmdGroup"></param>
        /// <param name="menuID"></param>
        /// <returns></returns>
        private CommandBar GetCommandBar(Guid guidCmdGroup, uint menuID, string commandBarName)
        {
            // Make sure the CommandBars collection is properly initialized, before we attempt to
            // use the IVsProfferCommands service.
            CommandBar menuBarCommandBar = ((CommandBars)ApplicationObject.CommandBars)[commandBarName];

            // Retrieve IVsProfferComands via DTE's IOleServiceProvider interface
            IOleServiceProvider sp = (IOleServiceProvider)ApplicationObject;
            Guid guidSvc = typeof(IVsProfferCommands).GUID;
            Object objService;
            sp.QueryService(ref guidSvc, ref guidSvc, out objService);
            IVsProfferCommands vsProfferCmds = (IVsProfferCommands)objService;
            return vsProfferCmds.FindCommandBar(IntPtr.Zero, ref guidCmdGroup, menuID) as CommandBar;
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