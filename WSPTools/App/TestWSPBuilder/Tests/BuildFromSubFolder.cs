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
    public class BuildFromSubFolder
    {
        private const string PROJECTNAME = @"BuildFromSubFolder";

        private const string _80BIN = @"\80\bin";
        private const string GAC = @"\GAC";
        private const string BINDEBUG = @"\bin\Debug";
        private const string BINRELEASE = @"\bin\release";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
            //TestInvironment.CleanUp(PROJECTNAME+_80BIN);
            //TestInvironment.CleanUp(PROJECTNAME+GAC);
            TestInvironment.CleanUp(PROJECTNAME+BINDEBUG);
            TestInvironment.CleanUp(PROJECTNAME+BINRELEASE);
        }


        [Test]
        public void Build80Bin()
        {
            string filename = TestInvironment.ProjectsPath + @"\" + PROJECTNAME + @"\manifest.xml";
            TestBuild(_80BIN, filename);
        }

        [Test]
        public void BuildGAC()
        {
            string filename = TestInvironment.ProjectsPath + @"\" + PROJECTNAME + @"\manifest.xml";
            TestBuild(GAC, filename);
        }

        [Test]
        public void BuildBinDebug()
        {
            string filename = TestInvironment.ProjectsPath + @"\" + PROJECTNAME + BINDEBUG + @"\manifest.xml";
            TestBuild(BINDEBUG, filename);
        }

        [Test]
        public void BuildBinRelease()
        {
            string filename = TestInvironment.ProjectsPath + @"\" + PROJECTNAME + BINRELEASE + @"\manifest.xml";
            TestBuild(BINRELEASE, filename);
        }

        private void TestBuild(string subFolder, string manifestPath)
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME + subFolder;
            wspBuilder.Build();

            Assert.IsTrue(File.Exists(manifestPath));
            ManifestHandler manifestHandler = new ManifestHandler(manifestPath);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            Assert.IsTrue(manifestHandler.Manifest.Assemblies[0].Location.Equals("WSPDemo.dll", StringComparison.InvariantCultureIgnoreCase));

        }
    }
}
