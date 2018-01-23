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
using Keutmann.SharePoint.WSPBuilder;
using Keutmann.SharePoint.WSPBuilder.Library;


namespace TestWSPBuilder
{
    [TestFixture]
    public class UnmanagedDLLs
    {
        private const string PROJECTNAME = @"UnmanagedDLLs";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }


        [Test]
        public void Build()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            //wspBuilder.Arguments = "";
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));

            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsTrue(manifestHandler.Manifest.Assemblies.Length == 1, "There should only be one assembly reference in the manifest.xml file!");
            Assert.IsTrue(manifestHandler.Manifest.Assemblies[0].Location.Equals("Unmanaged.Dll", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
