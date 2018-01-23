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
using Keutmann.SharePoint.WSPBuilder.Library;


namespace TestWSPBuilder
{
    [TestFixture]
    public class Deployment
    {
        private const string PROJECTNAME = "Deployment";

        private const string PACKAGENAME = "test100.wsp";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
        }

        [Test]
        public void Install()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments += " -buildwsp false -install "+PACKAGENAME;
            wspBuilder.Build();

            string packageFullname = wspBuilder.ProjectPath + @"\" + PACKAGENAME;

            Assert.IsTrue(Installer.IsAlreadyInstalledByPackage(packageFullname), "The package is not installed into sharepoint");
        }

        [Test]
        public void Uninstall()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            string packageFullname = wspBuilder.ProjectPath + @"\" + PACKAGENAME;

            if (!Installer.IsAlreadyInstalledByPackage(packageFullname))
            {
                this.Install();
            }

            wspBuilder.Arguments += " -buildwsp false -uninstall " + PACKAGENAME;
            wspBuilder.Build();

            Assert.IsFalse(Installer.IsAlreadyInstalledByPackage(packageFullname), "The package is still installed in the sharepoint server.");
        }

        [Test]
        public void Deploy()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments += " -buildwsp false -deploy true -wspname " + PACKAGENAME;
            wspBuilder.Build();

            string packageFullname = wspBuilder.ProjectPath + @"\" + PACKAGENAME;

            Assert.IsTrue(Installer.IsAlreadyInstalledByPackage(packageFullname), "The package is not deploy into sharepoint");
        }

        [Test]
        public void Retract()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            string packageFullname = wspBuilder.ProjectPath + @"\" + PACKAGENAME;

            if (!Installer.IsAlreadyInstalledByPackage(packageFullname))
            {
                this.Deploy();
            }

            wspBuilder.Arguments += " -buildwsp false -retract true -wspname " + PACKAGENAME;
            wspBuilder.Build();

            Assert.IsTrue(Installer.IsAlreadyInstalledByPackage(packageFullname), "The package is not installed into sharepoint");
        }

        [Test]
        public void Upgrade()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments += " -buildwsp false -install " + PACKAGENAME;
            wspBuilder.Build();

            string packageFullname = wspBuilder.ProjectPath + @"\" + PACKAGENAME;

            Assert.IsTrue(Installer.IsAlreadyInstalledByPackage(packageFullname), "The package is not installed into sharepoint");
        }
    }
}
