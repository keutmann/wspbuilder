// From the blog: http://www.pcreview.co.uk/forums/thread-1233244.php
// By "Doug" <dnlwhite@dtgnet.com>

#region namespace references
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace System.GACManagedAccess
{
    /// <summary>

    /// A map from HRESULTS to a .net type. Reference file

    /// CorError.h for a complete list.

    /// </summary>

    public enum GACErrorCodes : uint
    {
        /// <summary>

        /// This error indicates that during a bind, the assembly reference did 

        /// not match the definition of the assembly found. For strongly-named 

        /// assemblies, if any of the tuple (name, version, public key token, 

        /// culture) do not match when binding to an assembly using 

        /// Assembly.Load (or implicitly through type resolution), you will get this error. 

        /// For simply-named assemblies mismatches in the tuple (name, culture) 

        /// will cause a ref-def mismatch.

        /// </summary>

        FUSION_E_REF_DEF_MISMATCH = 0x80131040,

        /// <summary>

        /// Private (or "simply named") assemblies are restricted to live only under the 

        /// appbase directory for a given domain. This restriction is necessary to prevent 

        /// unintended sharing of the assembly. Since the identity of simply-named is 

        /// not unique, it is possible name collisions can occur, and therefore, 

        /// it is not safe to share the assembly across apps/domains. If a 

        /// simply-named assembly is ever loaded via Assembly.Load that does not appear 

        /// underneath the appbase, this error will be returned.

        /// </summary>

        FUSION_E_INVALID_PRIVATE_ASM_LOCATION = 0x80131041,



        /// <summary>

        /// When installing an assembly in the GAC, if a module (part of a 

        /// multi-file assembly) listed in the manifest is not found during 

        /// installation time, the install will fail with this error code. Modules 

        /// should be present alongside the manifest file when installing them into the GAC. 

        /// </summary>

        FUSION_E_ASM_MODULE_MISSING = 0x80131042,



        /// <summary>

        /// When installing multi-file assemblies into the GAC, the hash of each module is 

        /// checked against the hash of that file stored in the manifest. If the 

        /// hash of one of the files in the multi-file assembly does not match what is recorded 

        /// in the manifest, FUSION_E_UNEXPECTED_MODULE_FOUND will be returned. 

        /// 

        /// The name of the error, and the text description of it, are somewhat confusing. 

        /// The reason this error code is described this way is that the internally, 

        /// Fusion/CLR implements installation of assemblies in the GAC, by installing 

        /// multiple "streams" that are individually committed. 

        /// 

        /// Each stream has its hash computed, and all the hashes found 

        /// are compared against the hashes in the manifest, at the end of the installation. 

        /// Hence, a file hash mismatch appears as if an "unexpected" module was found. 

        /// </summary>

        FUSION_E_UNEXPECTED_MODULE_FOUND = 0x80131043,



        /// <summary>

        /// This error is returned when a simply-named assembly is encountered, 

        /// where a strongly-named one was expected. This error code could be returned, 

        /// for example, if you try to install a simply-named assembly into the GAC. 

        /// To strongly-name your assembly, use sn.exe. 

        /// </summary>

        FUSION_E_PRIVATE_ASM_DISALLOWED = 0x80131044,



        /// <summary>

        /// Both at bind time, and install-time, strong name signatures of assemblies are 

        /// validated. Every strongly-named assembly has a hash of the bits 

        /// recorded in the manifest (signed with the private key). If the hash of 

        /// the assembly found does not match that recorded in the manifest, signature 

        /// validation fails. 

        /// </summary>

        FUSION_E_SIGNATURE_CHECK_FAILED = 0x80131045,



        /// <summary>

        /// There are a variety of conditions which can trigger this error, most of 

        /// which are related to either poorly-formed assembly "display names" 

        /// (textual identities), or path-too-long conditions when translating an 

        /// assembly identity into a corresponding location on the file system. 

        /// </summary>

        FUSION_E_INVALID_NAME = 0x80131047,



        /// <summary>

        /// This error indicates that AppDomainSetup.DisallowCodeDownload has been set 

        /// on the domain. Setting his flag prevents any http:// download of 

        /// assemblies (or configuration files). ASP.NET sets this flag by default 

        /// because the internal usage of wininet/urlmon to download bits over 

        /// http:// is not supported under service processes. 

        /// </summary>

        FUSION_E_CODE_DOWNLOAD_DISABLED = 0x80131048,



        /// <summary>

        /// When the CLR is installed as part of the operating system 

        /// (such as in Windows Server 2003), uninstalling CLR or frameworks assemblies 

        /// is disallowed.

        /// </summary>

        FUSION_E_UNINSTALL_DISALLOWED = 0x80131049,
    }
}