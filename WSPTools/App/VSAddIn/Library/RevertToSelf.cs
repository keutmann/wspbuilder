using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace WSPTools.VisualStudio.VSAddIn.Library
{
    public class RevertToSelf : IDisposable
    {
        private WindowsImpersonationContext ctx = null;

        public WindowsIdentity Identity
        {
            get
            {
                return WindowsIdentity.GetCurrent();
            }
        }


        public RevertToSelf()
        {
            UseSelfIdentity();
        }

        public void UseSelfIdentity()
        {
            try
            {
                if (!WindowsIdentity.GetCurrent().IsSystem)
                {
                    ctx = WindowsIdentity.Impersonate(System.IntPtr.Zero);
                }
            }
            catch { }
        }

        public void ReturnToImpersonatingCurrentUser()
        {
            try
            {
                if (ctx != null)
                {
                    ctx.Undo();
                    ctx = null;
                }
            }
            catch { }
        }

        #region IDisposable Members

        public void Dispose()
        {
            ReturnToImpersonatingCurrentUser();
        }

        #endregion
    }
}
