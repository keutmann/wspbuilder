using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using Keutmann.SharePoint.WSPBuilder.Commands.Attributes;
using System.Diagnostics;
using System.ComponentModel;

namespace Keutmann.SharePoint.WSPBuilder.Commands
{
    public class CommandParser
    {
        private Dictionary<string, string> _argsCollection = null;

        public string[] Args { get; set; }

        public Dictionary<string, string> ArgsCollection
        {
            get
            {
                if (_argsCollection == null)
                {
                    _argsCollection = GetArgsCollection();
                }
                return _argsCollection;
            }
        }


        public CommandParser()
        {
        }

        public CommandParser(string[] args)
        {
            this.Args = args;
        }

        public static void Execute(string[] args)
        {
            BaseCommand action = null;
            if (args.Length == 0)
            {
                action = new Help();
            }
            else
            {
                CommandParser parser = new CommandParser(args);
                action = parser.GetAction();
            }

            action.Execute();
        }

        public BaseCommand GetAction()
        {
            BaseCommand commandInstance = null;

            if (this.Args.Length == 0)
            {
                commandInstance = new Help();
            }
            else
            {
                string opration = GetOperationName();

                Type commandType = GetOperationType(opration);
                if (commandType != null)
                {
                    commandInstance = (BaseCommand)Activator.CreateInstance(commandType);

                    InitializeClassProperties(commandType, commandInstance);
                }
                else
                {
                    throw new ApplicationException("Unknown operation.: " + opration);
                }
            }

            return commandInstance;
        }


        private void InitializeClassProperties(Type actionType, BaseCommand actionInstance)
        {
            Dictionary<string, PropertyInfo> classParameters = GetClassProperties(actionType);

            for (int i = 2; i < this.Args.Length; i++)
            {
                string rawArgument = this.Args[i];

                if (rawArgument.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                {
                    // Remove the "-" in front of the argument
                    string argumentName = rawArgument.Substring(1);

                    if (classParameters.ContainsKey(argumentName))
                    {
                        string argumentValue = null;
                        PropertyInfo classProperty = classParameters[argumentName];
                        object parsedValue = null;

                        if (IsNextArgumentAValue(i))
                        {
                            // Move the index one step ahead to the value
                            i++;
                            argumentValue = this.Args[i];
                            parsedValue = GetParsedValue(argumentValue, classProperty);
                        }
                        else
                        {
                            parsedValue = GetCustomDefaultValue(classProperty);
                        }

                        if (parsedValue != null)
                        {
                            classProperty.SetValue(actionInstance, parsedValue, null);
                        }

                        if (parsedValue == null && IsClassPropertyRequired(classProperty))
                        {
                            throw new ArgumentException(String.Format("Missing value for the {0} parameter!",rawArgument));
                        }
                    }
                    else
                    {
                        throw new ArgumentException(String.Format("The argument {0} is not available as parameter in this operation!", rawArgument));
                    }
                }
                else
                {
                    throw new ArgumentException(String.Format("The argument {0} is not a valid parameter!", rawArgument));
                }
            }
        }


        private object GetCustomDefaultValue(PropertyInfo classProperty)
        {
            object parsedValue = null;

            object[] customAttributes = classProperty.GetCustomAttributes(true);
            DefaultValueAttribute dvAttribute = GetCustomAttribute<DefaultValueAttribute>(customAttributes);
            if (dvAttribute != null)
            {
                parsedValue = dvAttribute.Value;
            }

            return parsedValue;
        }


        private bool IsClassPropertyRequired(PropertyInfo classProperty)
        {
            bool isRequired = false;
            object[] customAttributes = classProperty.GetCustomAttributes(true);
            Required required = (Required)GetCustomAttribute(customAttributes, "Required");
            if (required != null)
            {
                isRequired = true;
            }
            return isRequired;
        }


        private bool IsNextArgumentAValue(int index)
        {
            bool result = false;

            if (index + 1 < this.Args.Length)
            {
                string argument = this.Args[index + 1];
                result = !argument.StartsWith("-", StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }


        public Dictionary<string, PropertyInfo> GetClassProperties(Type actionType)
        {
            Dictionary<string, PropertyInfo> parameters = new Dictionary<string,PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            PropertyInfo[] properties = actionType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                parameters.Add(property.Name, property);
            }

            return parameters;
        }


        private object GetParsedValue(string rawValue, PropertyInfo property)
        {
            Type propType = property.PropertyType;
            object result = null;

            MethodInfo parseInfo = propType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (parseInfo != null)
            {
                try
                {
                    result = parseInfo.Invoke(null, new object[] { rawValue });
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(String.Format("Error in parameter -{0}. {1}", property.Name, ex.Message));
                }
            }
            else
            {
                if (propType.IsEnum)
                {
                    try
                    {
                        result = Enum.Parse(propType, rawValue, true);
                    }
                    catch 
                    {
                        throw new ArgumentException(String.Format("Error in parameter -{0}. The value is not valid: {1}", property.Name, rawValue));
                    }
                }
                else
                {
                    result = rawValue;
                }

            }
            return result;
        }


        private object GetCustomAttribute(object[] customAttributes, string name)
        {
            object attribute = null;

            foreach (object item in customAttributes)
            {
                if (item.GetType().Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    attribute = item;
                    break;
                }
            }

            return attribute;
        }


        private T GetCustomAttribute<T>(object[] customAttributes)
        {
            T attribute = default(T);

            foreach (object item in customAttributes)
            {
                if (item is T)
                {
                    attribute = (T)item;
                    break;
                }
            }

            return attribute;
        }

        private string GetOperationName()
        {
            string result = string.Empty;
            if (this.Args.Length > 0)
            {
                if ("-o".Equals(this.Args[0], StringComparison.OrdinalIgnoreCase))
                {
                    if (this.Args.Length > 1)
                    {
                        result = this.Args[1];
                    }
                    else
                    {
                        throw new ApplicationException("Missing operation command after the -o parameter.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Every operation must start with a -o parameter.");
                }
            }
            return result;
        }

        public Type GetOperationType(string operation)
        {
            Type result = null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in assemblies)
            {
                
                result = GetType(operation, asm);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        private Type GetType(string operation, Assembly sourceAsm)
        {
            Type result = null;
            Assembly asm = sourceAsm;
            result = FindType(operation, asm);

            if (result == null)
            {
                asm = Assembly.GetExecutingAssembly();
                result = FindType(operation, asm);
                if (result == null)
                {
                    asm = Assembly.GetCallingAssembly();
                    result = FindType(operation, asm);
                }
            }
            return result;
        }

        private Type FindType(string operation, Assembly asm)
        {
            Type result = null;
            if (asm != null)
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(BaseCommand)) && !type.IsAbstract)
                    {
                        if (type.Name.Equals(operation, StringComparison.OrdinalIgnoreCase))
                        {
                            result = type;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private Dictionary<string, string> GetArgsCollection()
        {
            Dictionary<string, string> coll = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int i = 0;
            while (i < this.Args.Length)
            {
                string key = this.Args[i].Substring(1);
                string value = (i + 1 < this.Args.Length) ? this.Args[i + 1] : string.Empty;
                coll.Add(key, value);
                i += 2;
            }

            return coll;
        }
    }
}
