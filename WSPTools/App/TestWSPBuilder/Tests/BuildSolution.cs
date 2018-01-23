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
    public class BuildSolution
    {
        private const string PROJECTNAME = "BuildSolution";

        private string ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME ;

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);

            //File.Copy(ProjectPath + @"\TestWSPBuilder.dll.config", Environment.CurrentDirectory + @"\TestWSPBuilder.dll.config", true);
        }

        [TearDown]
        public void Teardown()
        {
            //string configFile = Environment.CurrentDirectory+@"\TestWSPBuilder.dll.config";
            //if(File.Exists(configFile))
            //{
            //    File.Delete(configFile);
            //}
        }


        /// <summary>
        /// The bug was reported as a buildsolution, but can olso be defined tested with arguments.
        /// The AppConfig gets loaded. Have been internaly tested. Because it not easy to test with a app.config file in NUnit,
        /// when config file has to be loaded before NUnit starts.
        /// </summary>
        [Test]
        public void BuildSimple()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments = @" -BuildSolution true -cleanup false -buildddf true";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));


            ManifestHandler manifestHandler = new ManifestHandler(filename);
            AssemblyFileReference[] assemblies = manifestHandler.Manifest.Assemblies;
            Assert.IsNotNull(assemblies);
            Assert.IsTrue(DoMultiAssemblyExists(assemblies, @"Dummy.DLL") == 1, "There can not be more than one assembly in the same folder!");
        }

        [Test]
        public void BuildWithArguments()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments += @" -BuildSolution true -GACPath Project1\GAC -12Path Project2\12 -80Path Project1\80 -BinPath Project1\80\bin";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));


            ManifestHandler manifestHandler = new ManifestHandler(filename);
            AssemblyFileReference[] assemblies = manifestHandler.Manifest.Assemblies;
            Assert.IsNotNull(assemblies);
            Assert.IsTrue(DoMultiAssemblyExists(assemblies, @"Dummy.DLL") == 1, "There can not be more than one assembly in the same folder!");
        }

        private int DoMultiAssemblyExists(AssemblyFileReference[] assemblies, string location)
        {
            int count = 0;
            foreach (AssemblyFileReference assembly in assemblies)
            {
                if (assembly.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    count ++;
                }
            }
            return count;
        }
    }
}
