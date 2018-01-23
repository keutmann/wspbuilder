/* Program : TestWSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * This test requires NUnit 
 */
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using TestWSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Library;


namespace TestWSPBuilder
{
    [TestFixture]
    public class SatelliteAssemblies
    {
        private const string PROJECTNAME = "SatelliteAssemblies";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }

        [Test]
        public void Build()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments = "-cleanup false -buildcab true -TraceLevel Verbose";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            Assert.IsTrue(TestUtil.FileSize(filename) > 1000, "Manifest.xml is too small, please check!");

            ManifestHandler manifestHandler = new ManifestHandler(filename);
            AssemblyFileReference[] assemblies = manifestHandler.Manifest.Assemblies;
            Assert.IsNotNull(assemblies);
            Assert.IsTrue(DoAssemblyExists(assemblies, @"Dummy.DLL"));
            Assert.IsTrue(DoAssemblyExists(assemblies, @"WSPDemo.DLL"));
            Assert.IsTrue(DoAssemblyExists(assemblies, @"da-Dk\WSPDemo.Resources.DLL"));
            Assert.IsTrue(DoAssemblyExists(assemblies, @"en-GB\WSPDemo.Resources.DLL"));
        }

        private bool DoAssemblyExists(AssemblyFileReference[] assemblies, string location)
        {
            bool found = false;
            foreach (AssemblyFileReference assembly in assemblies)
            {
                if (assembly.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}
