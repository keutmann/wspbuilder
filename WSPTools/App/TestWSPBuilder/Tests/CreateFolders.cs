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
    public class CreateFolders
    {
        private const string PROJECTNAME = "CreateFolders";

        [SetUp]
        public void Setup()
        {
            TestInvironment.CleanUp(PROJECTNAME);
            string currentProjectPath = TestInvironment.ProjectsPath + @"\" + PROJECTNAME;
            DirectoryInfo projectDir = new DirectoryInfo(currentProjectPath);
            foreach (DirectoryInfo subDir in projectDir.GetDirectories())
            {
                subDir.Delete(true);
            }
        }

        [Test]
        public void Build()
        {
            WSPBuilderHandler wspBuilder = new WSPBuilderHandler(PROJECTNAME);
            wspBuilder.Arguments = "-createfolders true";
            wspBuilder.Build();

            string folder12Path = wspBuilder.ProjectPath + @"\12";
            string folder80Path = wspBuilder.ProjectPath + @"\80";
            string folderGACPath = wspBuilder.ProjectPath + @"\GAC";
            string folderTemplatePath = folder12Path + @"\Template";
            Assert.IsTrue(Directory.Exists(folder12Path));
            Assert.IsTrue(Directory.Exists(folder80Path));
            Assert.IsTrue(Directory.Exists(folderGACPath));
            Assert.IsTrue(Directory.Exists(folderTemplatePath));
        }
    }
}
