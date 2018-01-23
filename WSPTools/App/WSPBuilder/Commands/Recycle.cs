using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using Keutmann.SharePoint.WSPBuilder.SystemServices;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.Reflection;
using WSPTools.BaseLibrary.SystemServices;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    [Description("Recycles IIS Application Pools and timer services. If no parameters is specified then everything are recycled.")]
    public class Recycle : TraceLevelCommand
    {
        // -o Recycle
        // [-poolname]
        // [-allpools]
        // [-owstimer]
        // [-admtimer]
        // [-all]

        #region Properties

        [Description("Define the Application Pool to recycle.")]
        public string Name { get; set; }

        [Description("Recycles all IIS Applications Pools.")]
        public bool? AllPools { get; set; }

        [Description("Recycles everything.")]
        public bool? All { get; set; }

        [Description("Recycles the ows timer service.")]
        public bool? OwsTimer { get; set; }

        [Description("Recycles the administration timer service.")]
        public bool? AdmTimer { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        public override void Execute()
        {
            // Call base execute to ensure that base functionality is called if there are any.
            base.Execute();

            if (!String.IsNullOrEmpty(this.Name))
            {
                IIServerManager.RecycleApplicationPool(this.Name);
            }

            if (this.AllPools == true)
            {
                IIServerManager.RecycleApplicationPools();
            }

            if (this.OwsTimer == true)
            {
                WindowsServices.Restart(WindowsServices.Current.SPTimerName);
            }

            if (this.AdmTimer == true)
            {
                WindowsServices.Restart(WindowsServices.Current.SPAdminName);
            }

            if (this.All == true || NothingSpecified())
            {
                IIServerManager.RecycleApplicationPools();
                WindowsServices.Restart(WindowsServices.Current.SPTimerName);
                WindowsServices.Restart(WindowsServices.Current.SPAdminName);
            }
        }

        private bool NothingSpecified()
        {
            bool nothingSpecified = String.IsNullOrEmpty(this.Name)
                && this.All == null
                && this.AllPools == null
                && this.AdmTimer == null
                && this.OwsTimer == null;

            return nothingSpecified;
        }



 

        #endregion
    }
}
