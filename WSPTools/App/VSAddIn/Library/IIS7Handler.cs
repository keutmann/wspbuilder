using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Administration;

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class IIS7Handler
    {

        #region fields

        private DTEHandler _DTEInstance = null;
        private string _machineName = null;


        #endregion

        #region public properties

        public DTEHandler DTEInstance
        {
            get { return _DTEInstance; }
            set { _DTEInstance = value; }
        }

        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        #endregion

        public IIS7Handler(DTEHandler handler)
        {
            this.DTEInstance = handler;
            this.MachineName = Environment.MachineName;   
        }

        public void RecycleAppPools()
        {
            ServerManager manager = new ServerManager();

            foreach (ApplicationPool pool in manager.ApplicationPools)
            {
                if (pool.State == ObjectState.Started)
                {
                    this.DTEInstance.WriteBuildWindow(String.Format("Recycling {0}", pool.Name));
                    pool.Recycle();
                }
                else
                {
                    this.DTEInstance.WriteBuildWindow(String.Format("Cannot recycle {0} because of state: {1}", pool.Name, pool.State));
                }
            }
        }

    }
}
