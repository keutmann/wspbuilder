using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    public class ProgramInfo
    {
        #region Const

        #endregion

        #region Members

        private Assembly _executingAssembly = null;

        #endregion

        #region Properties



        public Assembly ExecutingAssembly
        {
            get
            {
                if (_executingAssembly == null)
                {
                    _executingAssembly = Assembly.GetExecutingAssembly();
                }
                return _executingAssembly;
            }
            set
            {
                _executingAssembly = value;
            }
        }

        #endregion

        #region Methods

        public ProgramInfo(Assembly asm)
        {
            this.ExecutingAssembly = asm;
        }

        private Attribute GetAttribute(Type attribType)
        {
            object[] attrs = ExecutingAssembly.GetCustomAttributes(attribType, true);
            if (attrs.Length > 0)
            {
                return (Attribute)attrs[0];
            }
            return null;
        }

        public string InfoString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine);

            AssemblyTitleAttribute titleAttrib = (AssemblyTitleAttribute)GetAttribute(typeof(AssemblyTitleAttribute));
            sb.Append(titleAttrib.Title + Environment.NewLine);

            AssemblyFileVersionAttribute versionAttrib = (AssemblyFileVersionAttribute)GetAttribute(typeof(AssemblyFileVersionAttribute));
            if (versionAttrib != null)
            {
                sb.Append("Version: " + versionAttrib.Version.ToString() + Environment.NewLine);
            }

            AssemblyDescriptionAttribute descriptionAttrib = (AssemblyDescriptionAttribute)GetAttribute(typeof(AssemblyDescriptionAttribute));
            sb.Append(descriptionAttrib.Description + Environment.NewLine);

            AssemblyCompanyAttribute companyAttrib = (AssemblyCompanyAttribute)GetAttribute(typeof(AssemblyCompanyAttribute));
            sb.Append(companyAttrib.Company + Environment.NewLine);

            AssemblyCopyrightAttribute copyrightAttrib = (AssemblyCopyrightAttribute)GetAttribute(typeof(AssemblyCopyrightAttribute));
            sb.Append(copyrightAttrib.Copyright + Environment.NewLine);

            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        public void Print()
        {
            Console.Write(this.InfoString());
        }

        #endregion
    }
}
