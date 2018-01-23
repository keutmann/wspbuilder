/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2009 Juli
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.ServiceProcess;
using WSPTools.VisualStudio.VSAddIn;

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class WindowsServices
    {
        public const string SPTIMERV3_NAME = "SPTimerV3";

        private DTEHandler DTEInstance = null;

        public WindowsServices(DTEHandler dteHandler)
        {
            this.DTEInstance = dteHandler;
        }

        public void Stop(ServiceController sc)
        {
            
            string name = typeof(WindowsServices).Name;

            TimeSpan timeout = new TimeSpan(0, 0, 30);

            //check the status of the service
            if (sc.Status == ServiceControllerStatus.Running)
            {
                //make sure the service is stoppable
                if (sc.CanStop)
                {
                    DTEInstance.WriteBuildWindow("Restarting the " + sc.DisplayName + " service.");

                    //stop SPTimerV3
                    sc.Stop();

                    try
                    {
                        //wait 30 seconds for SPTImerV3 to stop
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                        DTEInstance.WriteBuildWindow(sc.DisplayName + " service stopped successfully.");
                    }
                    catch (System.ServiceProcess.TimeoutException)
                    {
                        DTEInstance.WriteBuildWindow(sc.DisplayName + " service did not respond to the stop command in a timely fashion.");
                    }
                }
                else
                {
                    DTEInstance.WriteBuildWindow(sc.DisplayName + " service cannot be stopped.");
                }
            }
        }

        public void Start(ServiceController sc)
        {
            TimeSpan timeout = new TimeSpan(0, 0, 30);

            string name = typeof(WindowsServices).Name;
         
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                //start SPTimerV3
                sc.Start();

                try
                {
                    //wait 30 seconds for SPTimerV3 to start
                    sc.WaitForStatus(ServiceControllerStatus.Running, timeout);

                    DTEInstance.WriteBuildWindow(sc.DisplayName + " service started successfully.");
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    DTEInstance.WriteBuildWindow(sc.DisplayName + " service did not respond to the start command in a timely fashion.");
                }
            }
        }

        public void Restart(string serviceName)
        {
            //create instance of service controller
            ServiceController sc = new ServiceController(serviceName);
            try
            {
                Stop(sc);
                Start(sc);
            }
            finally
            {
                //cleanup
                sc.Close();
                sc = null;
            }
        }
    }
}
