using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using Keutmann.SharePoint.WSPBuilder.Commands.Attributes;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    [Description("Shows the help for this application.")]
    public class Help : BaseCommand
    {
        [Description("Specify the name of the operation to display help on.")]
        public string Operation { get; set; }



        public override void Execute()
        {
            if (String.IsNullOrEmpty(this.Operation))
            {
                ListOperations();
            }
            else
            {
                DisplayOperationHelp();
            }
        }

        private void DisplayOperationHelp()
        {
            CommandParser parser = new CommandParser();
            Type actionType = parser.GetOperationType(this.Operation);

            Console.WriteLine("WSPBuilder.exe -o "+this.Operation);

            object[] classAttribs = actionType.GetCustomAttributes(false);
            foreach (object obj in classAttribs)
            {
                if (obj is DescriptionAttribute)
                {
                    string classDescription = ((DescriptionAttribute)obj).Description;
                    Console.WriteLine(classDescription);
                }
            }

            foreach (PropertyInfo propInfo in actionType.GetProperties())
            {
                bool required = false;
                string parameterName = propInfo.Name;
                string description = string.Empty;

                object[] attribs = propInfo.GetCustomAttributes(false);
                foreach (object obj in attribs)
                {

                    if (obj is Required)
                    {
                        required = true;
                    }

                    if (obj is DisplayNameAttribute)
                    {
                        parameterName = ((DisplayNameAttribute)obj).DisplayName;
                    }

                    if (obj is DescriptionAttribute)
                    {
                        description = ((DescriptionAttribute)obj).Description;
                    }
                }

                if (required)
                {
                    Console.WriteLine(String.Format("-{0}\t: (Required) {1}", parameterName, description));
                }
                else
                {
                    Console.WriteLine(String.Format("-{0}\t: (Optional) {1}", parameterName, description));
                }
            }
        }

        private static void ListOperations()
        {
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Operations [-o]");
            Console.WriteLine("");

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                ListActions(asm);
            }

            Console.WriteLine("");
        }

        private static void ListActions(Assembly asm)
        {
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BaseCommand)) 
                    && !type.IsAbstract
                    && type != typeof(ConsoleCommand))
                {
                    Console.WriteLine("  " + type.Name);
                }
            }
        }

    }

}
