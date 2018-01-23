using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Commands.Attributes;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    public class TraceLevelCommand : ConsoleCommand
    {
        private TraceLevel? _tracelevel = null;


        [DisplayName("-TraceLevel [Off|Error|Warning|Info|Verbose] (Defaut value is Info)")]
        [Description("The trace level switch setting for the application. It's possible to add more Trace listeners in WSPBuilder.exe.config file.")]
        public TraceLevel TraceLevel
        {
            get
            {
                if (_tracelevel == null)
                {
                    _tracelevel = TraceLevel.Info;
                }
                return (TraceLevel)_tracelevel;
            }
            set 
            { 
                _tracelevel = value; 
            }
        }

        public override void Execute()
        {
            Config.Current.TraceLevel = TraceLevel;
            base.Execute();
        }

    }
}
