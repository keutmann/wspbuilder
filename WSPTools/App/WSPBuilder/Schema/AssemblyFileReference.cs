/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 * 
 * This class extends the AssemblyFileReference class (wss.cs) with the AssemblyObject attibute.
 * This is used to save the Assembly object when loaded.
 * 
 * ----------------------------------------------------------
 * 2008-04-06 IsManagedAssembly added
 * 2008-04-06 Mono.Cecil Assembly logic added
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using Mono.Cecil;
using Keutmann.SharePoint.WSPBuilder.WSP;
using Keutmann.SharePoint.WSPBuilder.Library;
using Keutmann.SharePoint.WSPBuilder.Framework.IO;
using Keutmann.SharePoint.WSPBuilder.Commands;

public partial class AssemblyFileReference 
{
    private AssemblyDefinition _assemblyObject = null;
    private FileInfo _fileHandle = null;
    private bool _managedAssembly = true;
    private bool _resourceAssembly = false;
    private bool _referenceAssembly = false;

    [XmlIgnore()]
    public AssemblyDefinition AssemblyObject
    {
        get { return _assemblyObject; }
        set { _assemblyObject = value; }
    }

    [XmlIgnore()]
    public bool ManagedAssembly
    {
        get { return _managedAssembly; }
        set { _managedAssembly = value; }
    }

    [XmlIgnore()]
    public bool ResourceAssembly
    {
        get { return _resourceAssembly; }
        set { _resourceAssembly = value; }
    }

    /// <summary>
    /// Specifies that the assembly is not to be reflected on and no security policy is created.
    /// </summary>
    [XmlIgnore()]
    public bool ReferenceAssembly
    {
        get { return _referenceAssembly; }
        set { _referenceAssembly = value; }
    }

    [XmlIgnore()]
    public FileInfo FileHandle
    {
        get { return _fileHandle; }
        set { _fileHandle = value; }
    }

    public AssemblyFileReference()
    {
    }

    public AssemblyFileReference(AssemblyInfo assemblyInfo, SolutionDeploymentTargetType deploymentTarget)
    {
        this.DeploymentTarget = deploymentTarget;
        this.DeploymentTargetSpecified = true;
        this.Location = assemblyInfo.Key;
        this.AssemblyObject = null;
        this.FileHandle = assemblyInfo.FileHandle;
        this.ManagedAssembly = assemblyInfo.Managed;
        this.ResourceAssembly = assemblyInfo.Resource;
        this.ReferenceAssembly = assemblyInfo.Reference;
    }

    public void Move(Extractwsp handler)
    {
        Log.Verbose("Moving assembly file: " + this.Location);

        string sourcePath = Path.Combine(handler.SourcePath, this.Location);
        string targetPath = Path.Combine(handler.TargetPath, @"GAC\" + this.Location);
        if (this.DeploymentTargetSpecified)
        {
            if (this.DeploymentTarget == SolutionDeploymentTargetType.WebApplication)
            {
                targetPath = Path.Combine(handler.TargetPath, @"80\bin\" + this.Location);
            }
        }

        FileSystem.Move(sourcePath, targetPath, handler.Overwrite);
    }

}
