/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2008
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
#endregion

namespace Keutmann.SharePoint.WSPBuilder.Library
{


    public static class AssemblyStore
    {
        public const int COR_E_ASSEMBLYEXPECTED = -2147024885;



        static Dictionary<string, AssemblyType> asmStore = new Dictionary<string, AssemblyType>();

        public class ClassType
        {
            public string BaseName;
            public string BaseAssembly;
        }

        public class AssemblyType
        {
            public Dictionary<string, ClassType> Types = new Dictionary<string, ClassType>();
        }


        public static bool IsManagedAssembly(string filename)
        {
            bool isAsmbly = true;
            try
            {
                AssemblyName.GetAssemblyName(filename);
            }
            catch (BadImageFormatException imageEx)
            {
                int hrResult = Marshal.GetHRForException(imageEx);
                isAsmbly = (hrResult != COR_E_ASSEMBLYEXPECTED);
            }

            return isAsmbly;
        }


        public static AssemblyDefinition GetAssembly(string filename, string dllReferencePath)
        {
            AssemblyDefinition assembly = null;
            try
            {

                //Log.Verbose("Loading assembly: " + filename);
                assembly = AssemblyFactory.GetAssembly(filename);
                CustomAssemblyResolver resolver = new CustomAssemblyResolver();
                resolver.AddSearchDirectory(dllReferencePath);
                assembly.Resolver = resolver;

                Load(assembly);
            }
            catch (Exception ex)
            {
                Log.Error("Not able to load and reflect on assembly: " + filename+ ". However the assembly will still be included in the WSP package!");
#if DEBUG
                Log.Error(ex.Message);
                Log.Verbose(ex.StackTrace);
#endif
            }

            return assembly;
        }

        public static bool IsType(CustomAssemblyResolver resolver, string asmName, string className, Type targetType)
        {
            if (targetType.FullName.Equals(className))
            {
                return true;
            }

            ClassType classType = AssemblyStore.GetClassType(resolver, asmName, className);
            if (classType != null)
            {
                return IsType(resolver, classType.BaseAssembly, classType.BaseName, targetType);
            }
            return false;
        }

        public static ClassType GetClassType(CustomAssemblyResolver resolver, string asmName, string className)
        {
            AssemblyType asmType = null;
            if (asmStore.ContainsKey(asmName))
            {
                asmType = asmStore[asmName];
            }

            if (asmType == null)
            {
                asmType = Load(resolver.Resolve(asmName));
                resolver.Clear();
                GC.Collect();
            }

            ClassType type = null;
            if (asmType.Types.ContainsKey(className))
            {
                type = asmType.Types[className];
            }
            return type;
        }

        public static AssemblyType Load(AssemblyDefinition assembly)
        {
            AssemblyType asmType = null;
            if (asmStore.ContainsKey(assembly.Name.FullName))
            {
                asmType = asmStore[assembly.Name.FullName];
            }

            if (asmType == null)
            {
                asmType = new AssemblyType();
                foreach (TypeDefinition subType in assembly.MainModule.Types)
                {
                    if (subType.FullName != null)
                    {
                        if (subType.BaseType != null)
                        {
                            ClassType type = new ClassType();

                            type.BaseName = subType.BaseType.FullName;

                            if (subType.BaseType.Scope is AssemblyNameReference)
                            {
                                AssemblyNameReference subAssemblyReference = (AssemblyNameReference)subType.BaseType.Scope;
                                type.BaseAssembly = subAssemblyReference.FullName;
                            }
                            else
                            {
                                if (subType.BaseType.Scope is Mono.Cecil.ModuleDefinition)
                                {
                                    type.BaseAssembly = assembly.Name.FullName;
                                }
                            }

                            asmType.Types.Add(subType.FullName, type);
                        }
                    }
                }
                asmStore.Add(assembly.Name.FullName, asmType);
            }
            return asmType;
        }

    }
}