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
    public class SafeControls
    {
        private const string PROJECTNAME = @"SafeControls";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }


        [Test]
        public void BuildExpandFalse()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments += " -ExpandTypes false";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            //Assert.IsTrue(TestUtil.FileSize(filename) > 1000, "Manifest.xml is too small, please check!");

            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            foreach (SafeControlDefinition safeControl in manifestHandler.Manifest.Assemblies[0].SafeControls)
            {
                if (safeControl.TypeName.Equals("AbstractDummyControl", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ApplicationException("The SafeControl AbstractDummyControl should not be in manifest: " + safeControl.TypeName);
                }
                if (safeControl.TypeName.Equals("InterfaceDummy", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ApplicationException("The SafeControl should not be in manifest: " + safeControl.TypeName);
                }
                if (safeControl.TypeName.Equals("WindowsUserControl", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ApplicationException("The SafeControl should not be in manifest: " + safeControl.TypeName);
                }

                if (safeControl.TypeName.Equals("FormControl", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ApplicationException("The SafeControl should not be in manifest: " + safeControl.TypeName);
                }
            }
        }

        [Test]
        public void BuildExpandTrue()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments += " -ExpandTypes true";
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            wspBuilder.Build();

            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            Assert.IsTrue(TestUtil.FileSize(filename) > 1000, "Manifest.xml is too small, please check!");

            ManifestHandler manifestHandler = new ManifestHandler(filename);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].SafeControls.Length >= 9);

        }

    }
}
