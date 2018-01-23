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
    public class BuildAppResources
    {
        private const string PROJECTNAME = @"BuildAppResources";

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

            
            Assert.IsNotNull(manifestHandler.Manifest.ApplicationResourceFiles);
            Assert.IsNotNull(manifestHandler.Manifest.ApplicationResourceFiles.Items);
            Assert.AreEqual(manifestHandler.Manifest.ApplicationResourceFiles.Items.Length, 4);

            int countWPResources = 0;
            int countGlobalResouces = 0;
            ApplicationResourceFileDefinitions defs = manifestHandler.Manifest.ApplicationResourceFiles;
            foreach (object item in defs.Items)
            {
                if (item is App_GlobalResourceFileDefinition)
                {
                    App_GlobalResourceFileDefinition globalItem = item as App_GlobalResourceFileDefinition;
                    countGlobalResouces++;
                }
                else
                {
                    ApplicationResourceFileDefinition appResourceFileDef = item as ApplicationResourceFileDefinition;
                    countWPResources++;
                }                
            }

            Assert.AreEqual(countGlobalResouces, 3);
            Assert.AreEqual(countWPResources, 1);
        }
    }
}
