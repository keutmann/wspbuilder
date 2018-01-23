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
using System.Security;


namespace TestWSPBuilder
{
    [TestFixture]
    public class PermissionSet
    {
        private const string PROJECTNAME = @"PermissionSet";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }

        [Test]
        public void DefaultSetup()
        {

            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);

            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission);
        }

        [Test]
        public void PermissionSetLevel_None()
        {

            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);

            wspBuilder.Arguments += " -PermissionSetLevel none";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission);

            Assert.IsTrue(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission.Length >= 3, "There are missing permissions from the manifest.xml");
        }

        [Test]
        public void PermissionSetLevel_Minimal()
        {

            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);

            wspBuilder.Arguments += " -PermissionSetLevel miniMal";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission);

            Assert.IsTrue(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission.Length >= 5, "There are missing permissions from the manifest.xml");
        }

        [Test]
        public void PermissionSetLevel_Medium()
        {

            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);

            wspBuilder.Arguments += " -PermissionSetLevel medium";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet);
            Assert.IsNotNull(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission);

            Assert.IsTrue(manifestHandler.Manifest.CodeAccessSecurity[0].PermissionSet.IPermission.Length >= 12, "There are missing permissions from the manifest.xml");
        }


    }
}
