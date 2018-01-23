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
    public class ReplacementParameters
    {
        private const string PROJECTNAME = "ReplacementParameters";

        private string ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME ;

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }

        [Test]
        public void AssemblyFullName()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments = @" -cleanup false -projectassembly ""GAC\\Dummy.dll""";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));

            //ManifestHandler manifestHandler = new ManifestHandler(filename);
            //AssemblyFileReference[] assemblies = manifestHandler.Manifest.Assemblies;
            //Assert.IsNotNull(assemblies);
            //Assert.IsTrue(DoMultiAssemblyExists(assemblies, @"Dummy.DLL") == 1, "There can not be more than one assembly in the same folder!");
        }
    }
}
