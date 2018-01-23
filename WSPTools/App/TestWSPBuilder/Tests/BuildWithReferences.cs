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
    public class BuildWithReferences
    {
        private const string PROJECTNAME = "BuildWithReferences";

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
            wspBuilder.Arguments = @" -cleanup false";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            // Run
            wspBuilder.Build();

            // Test
            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));

            
            ManifestHandler manifestHandler = new ManifestHandler(filename);
            AssemblyFileReference[] assemblies = manifestHandler.Manifest.Assemblies;
            Assert.IsNotNull(assemblies);
            foreach (AssemblyFileReference assmRef in assemblies)
            {
                if ("Dummy.dll".EndsWith(assmRef.Location, StringComparison.OrdinalIgnoreCase))
                {
                    if (assmRef.SafeControls != null && assmRef.SafeControls.Length > 0)
                    {
                        throw new ApplicationException("Dummy.dll should not have SafeControls in manifest!");
                    }
                }
                if ("WSPDemo.dll".EndsWith(assmRef.Location, StringComparison.OrdinalIgnoreCase))
                {
                    if (assmRef.SafeControls == null && assmRef.SafeControls.Length == 0)
                    {
                        throw new ApplicationException("WSPDemo.dll should have SafeControls in manifest!");
                    }
                }

            }
            
        }


    }
}
