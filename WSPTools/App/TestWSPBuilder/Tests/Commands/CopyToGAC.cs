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


namespace TestWSPBuilder.Commands
{
    [TestFixture]
    public class CopyToGAC
    {
        private const string PROJECTNAME = "CopyToGAC";

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

        [Test]
        public void InvalidParameters()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            // Run First test
            string[] args = new string[] { "-o", "CopyToGAC", "raceLevel", "Verbose" };
            int error = wspBuilder.BuildCommand(args);

            // Test
            Assert.IsTrue(error == 1);

            // Run First test
            args = new string[] { "-o", "CopyToGAC", "-traceLevel", "sdfsd"};
            error = wspBuilder.BuildCommand(args);

            // Test
            Assert.IsTrue(error == 1);

        }



        [Test]
        public void InstallAssembly()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            string[] args = new string[] {"-o", "CopyToGAC", "-traceLevel", "Verbose" };
            wspBuilder.ProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;

            // Run
            int error = wspBuilder.BuildCommand(args);

            // Test
            Assert.IsTrue(error == 0);
        }


    }
}
