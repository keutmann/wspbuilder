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
    public class ManifestConfig
    {
        private const string PROJECTNAME = @"Manifest.Config";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }


        [Test]
        public void Build()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);

            // Run
            wspBuilder.Build();

            // Test
            string filename = wspBuilder.ProjectPath + @"\manifest.xml";
            Assert.IsTrue(File.Exists(filename));
            ManifestHandler manifestHandler = new ManifestHandler(filename);

            Assert.AreEqual("TitleTest", manifestHandler.Manifest.Title);
            Assert.AreEqual("DescriptionTest", manifestHandler.Manifest.Description);
            Assert.AreEqual("Dummy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5", manifestHandler.Manifest.ReceiverAssembly);
            Assert.AreEqual("Keutmann.Demo.Dummy.DummyControl", manifestHandler.Manifest.ReceiverClass);
            Assert.AreEqual(ResetWebServerModeOnUpgradeAttr.Recycle, manifestHandler.Manifest.ResetWebServerModeOnUpgrade);

            Assert.IsNotNull(manifestHandler.Manifest.Assemblies);
            Assert.AreEqual(manifestHandler.Manifest.Assemblies.Length, 1);
            Assert.IsNotNull(manifestHandler.Manifest.Assemblies[0].BindingRedirects);
            Assert.AreEqual(manifestHandler.Manifest.Assemblies[0].BindingRedirects.Length, 3);

            //int countWPResources = 0;
            //int countGlobalResouces = 0;
            //ApplicationResourceFileDefinitions defs = manifestHandler.Manifest.ApplicationResourceFiles;
            //foreach (object item in defs.Items)
            //{
            //    if (item is App_GlobalResourceFileDefinition)
            //    {
            //        App_GlobalResourceFileDefinition globalItem = item as App_GlobalResourceFileDefinition;
            //        countGlobalResouces++;
            //    }
            //    else
            //    {
            //        ApplicationResourceFileDefinition appResourceFileDef = item as ApplicationResourceFileDefinition;
            //        countWPResources++;
            //    }                
            //}

            //Assert.AreEqual(countGlobalResouces, 3);
            //Assert.AreEqual(countWPResources, 1);
        }
    }
}
