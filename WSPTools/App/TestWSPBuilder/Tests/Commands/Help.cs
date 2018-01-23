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
    public class Help
    {

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void ShowHelp()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler("");
            wspBuilder.Arguments = "-help";

            // Run
            int error = wspBuilder.Build();

            // Run First test
            //string[] args = new string[] { "help" };
            //int error = wspBuilder.BuildCommand(args);

            // Test
            Assert.IsTrue(error == 0);
        }

        [Test]
        public void ShowHelpVerion2()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler("");

            // Run First test
            string[] args = new string[] { "-o", "Help" };
            int error = wspBuilder.BuildCommand(args);

            // Test
            Assert.IsTrue(error == 0);
        }
        
        [Test]
        public void ShowExtractwsp()
        {
            // Setup
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler("");

            // Run First test
            string[] args = new string[] { "-o", "Help", "-operation", "Extractwsp" };
            int error = wspBuilder.BuildCommand(args);

            // Test
            Assert.IsTrue(error == 0);
        }

    }
}
