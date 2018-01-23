#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE;
#endregion

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class Log
    {
        public const string NEWLINE = "\r\n";

        private object _classObject = null;

        Log()
        {
            try
            {
                string filename = "C:\\Temp\\WSPAddin" + DateTime.Now.ToString("yyMMdd_hhmmss") + ".log";
                TextWriterTraceListener tr2 = new TextWriterTraceListener(File.CreateText(filename));
                Debug.Listeners.Clear();
                Debug.Listeners.Add(tr2);
                Debug.AutoFlush = true;
            }
            catch 
            {
                // Ignore error setting up the log - I think this will avoid the vista UAC issue
            }
        }

        private void _write(string text)
        {
            Debug.Write(text + NEWLINE);
        }

        private void _write(object classObject, string text)
        {
            if (classObject != this._classObject)
            {
                Log.Write("--- New Class instance: " + classObject.ToString() + " -----------");
                this._classObject = classObject;
            }
            Debug.Write(text + NEWLINE);
        }

        public static void Write(string text)
        {
            Instance._write(text);
        }

        public static void Write(object classObject, string text)
        {
            Instance._write(classObject, text);
        }


        public static void Error(Exception ex)
        {
#if DEBUG
            Instance._write(ex.Message);
            Instance._write(ex.StackTrace); 
#endif
        }


        public static void DumpToOutput(DTEHandler handler)
        {
            //CommandBars bars = (CommandBars)handler.Application.CommandBars;

            //string buildName = CommandBarStrings.GetName("Output", handler.Application);
            ////OutputWindowPane output = handler.ApplicationOutputWindow.OutputWindowPanes.Item("output");

            
            //foreach (CommandBar bar in bars)
            //{
            //    output.OutputString("Name: " + bar.NameLocal + " : ID: " + bar.Id + Environment.NewLine);
            //}
        }

        public static Log Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Log instance = new Log();
        }

    }
}