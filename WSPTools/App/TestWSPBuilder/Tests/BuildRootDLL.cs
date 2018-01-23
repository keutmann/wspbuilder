/* Program : TestWSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2009 Juli
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
    public class BuildRootDLL
    {
        private const string PROJECTNAME = "BuildRootDLL";

        private string ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME ;

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }

        [TearDown]
        public void Teardown()
        {
        }


        /// <summary>
        /// </summary>
        [Test]
        public void BuildSimple()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments = @" -cleanup false -TraceLevel Verbose";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            // Run
            wspBuilder.Build();

            // Test
            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));

            
            ManifestHandler manifestHandler = new ManifestHandler(filename);
            AssemblyFileReference[] assemblies = manifestHandler.Manifest.Assemblies;
            Assert.IsNull(assemblies, "There should not be any dlls in the manifest.xml!");
        }


    }
}
