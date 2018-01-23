/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class HelpHandler
    {
        #region Const

        private const string LINEBREAK = "\r\n";
        public const string HELP = "help";

        #endregion

        #region Members

        private Assembly _executingAssembly = null;

        private string _help = string.Empty;

        #endregion 

        #region Properties


        [DisplayName("-Help [Argument|Overview|Full] (Overview is default)")]
        [Description("Use the help to show detail description of the arguments.")]
        public string Help
        {
            get
            {
                if (string.IsNullOrEmpty(_help))
                {
                    _help = Config.Current.GetString(HELP, "OverView");
                }
                return _help;
            }
            set
            {
                _help = value;
            }
        }


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
        }

        #endregion

        #region Methods

        private Attribute GetAttribute(Type attribType)
        {
            object[] attrs = ExecutingAssembly.GetCustomAttributes(attribType, true);
            if (attrs.Length > 0)
            {
                return (Attribute)attrs[0];
            }
            return null;
        }

        private string HelpDescription(string argument)
        {
            SortedDictionary<string, string> arguments = new SortedDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            StringBuilder sb = new StringBuilder();
            sb.Append("Arguments --------------------------------- " + LINEBREAK);

            bool validargument = false;
            bool overview = (argument.Equals("full", StringComparison.InvariantCultureIgnoreCase) ||
                            argument.Equals("overview", StringComparison.InvariantCultureIgnoreCase));

            foreach (Type type in ExecutingAssembly.GetTypes())
            {
                string category = string.Empty;
                object[] classAttribs = type.GetCustomAttributes(false);
                //foreach (object obj in classAttribs)
                //{
                //    if (obj is CategoryAttribute)
                //    {
                //        category = ((CategoryAttribute)obj).Category;
                //    }
                //}

                foreach (PropertyInfo propInfo in type.GetProperties())
                {
                    string displayName = string.Empty;
                    string description = string.Empty;

                    object[] attribs = propInfo.GetCustomAttributes(false);
                    foreach (object obj in attribs)
                    {

                        if (obj is CategoryAttribute)
                        {
                            category = ((CategoryAttribute)obj).Category;
                        }

                        if (obj is DisplayNameAttribute)
                        {
                            displayName = ((DisplayNameAttribute)obj).DisplayName;
                        }

                        if (obj is DescriptionAttribute)
                        {
                            description = ((DescriptionAttribute)obj).Description;
                        }
                    }

                    if (overview)
                    {
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            string descr = string.Empty;
                            if (argument.Equals("full", StringComparison.InvariantCultureIgnoreCase))
                            {
                                descr = description;
                            }

                            if (!arguments.ContainsKey(displayName))
                            {
                                arguments.Add(displayName, descr);
                            }
                            else
                            {
                                //throw new ApplicationException("Field already exist! " + displayName);
                            }

                            validargument = true;
                        }
                    }
                    else
                    {
                        if (displayName.StartsWith("-" + argument, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Write out only this argument
                            arguments.Add(displayName, description);

                            validargument = true;
                        }
                    }
                }
            }

            if (!validargument)
            {
                throw new ApplicationException("Invalid value '"+ Help +"' for the Help argument.");
            }

            // Write out every argument
            foreach (string key in arguments.Keys)
            {
                sb.Append(LINEBREAK);
                sb.Append(key + LINEBREAK);
                sb.Append(arguments[key] + LINEBREAK);
            }


            sb.Append(LINEBREAK);
            sb.Append(LINEBREAK);

            //// Write out every argument
            //foreach (string key in arguments.Keys)
            //{
            //    string val = key.Substring(1, key.IndexOf(" "));
            //    sb.Append("<add key=\"" + val + "\" value=\"\" />" + LINEBREAK);
            //}

            //sb.Append(LINEBREAK);
            //sb.Append(LINEBREAK);

            return sb.ToString();
        }

        public string Copyleft()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(LINEBREAK);

            AssemblyTitleAttribute titleAttrib = (AssemblyTitleAttribute)GetAttribute(typeof(AssemblyTitleAttribute));
            sb.Append(titleAttrib.Title + LINEBREAK + LINEBREAK);

            AssemblyFileVersionAttribute versionAttrib = (AssemblyFileVersionAttribute)GetAttribute(typeof(AssemblyFileVersionAttribute));
            if (versionAttrib != null)
            {
                sb.Append("Version: " + versionAttrib.Version.ToString() + LINEBREAK);
            }

            AssemblyCompanyAttribute companyAttrib = (AssemblyCompanyAttribute)GetAttribute(typeof(AssemblyCompanyAttribute));
            sb.Append(companyAttrib.Company + LINEBREAK);
            AssemblyCopyrightAttribute copyrightAttrib = (AssemblyCopyrightAttribute)GetAttribute(typeof(AssemblyCopyrightAttribute));
            sb.Append(copyrightAttrib.Copyright + LINEBREAK);

            return sb.ToString();
        }

        public string Print()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Copyleft());

            sb.Append(LINEBREAK);

            AssemblyDescriptionAttribute descriptionAttrib = (AssemblyDescriptionAttribute)GetAttribute(typeof(AssemblyDescriptionAttribute));
            sb.Append(descriptionAttrib.Description + LINEBREAK);

            sb.Append(LINEBREAK);

            sb.Append(HelpDescription(Help));

            sb.Append(LINEBREAK + "Examples ----------------------------------" + LINEBREAK);

            sb.Append(LINEBREAK + @"WSPBuilder -Createfolders true" + LINEBREAK);
            sb.Append(LINEBREAK + @"WSPBuilder -SolutionPath C:\MyProject -Outputpath C:\MySolution" + LINEBREAK);
            sb.Append(LINEBREAK + @"WSPBuilder -ExpandTypes true" + LINEBREAK);

            sb.Append(LINEBREAK);

            return sb.ToString();
        }

        #endregion

    }
}
