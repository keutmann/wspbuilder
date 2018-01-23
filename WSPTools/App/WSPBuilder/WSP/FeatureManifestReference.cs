/* Program : WSPBuilder
 * Created by: Carsten Keutmann
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Keutmann.SharePoint.WSPBuilder.Library;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Keutmann.SharePoint.WSPBuilder.WSP
{
    public partial class SolutionHandler
    {
        #region Const

        private const string INCLUDEFEATURES = "Includefeatures";

        #endregion

        #region Members

        // A list of every file defined by a feature.xml elementFile tag.
        private List<String> ElementFiles = new List<string>();

        private bool? _includeFeatures = null;

        #endregion

        #region Properties

        [DisplayName("-Includefeatures [True|False] (Default is true)")]
        [Description("If true the features gets included into the WSP file.")]
        public bool IncludeFeatures
        {
            get
            {
                if (_includeFeatures == null)
                {
                    _includeFeatures = Config.Current.GetBool(INCLUDEFEATURES, true);
                }
                return (bool)_includeFeatures;
            }
            set
            {
                _includeFeatures = value;
            }
        }


        #endregion

        #region Methods

        private bool DoLocationExist(List<FeatureManifestReference> featureManifestReferenceList, string location)
        {
            if (!Config.Current.BuildSolution)
            {
                // Do not check for Existing location files, because its not possible in a project only build.
                return false;
            }

            bool found = false;
            foreach (FeatureManifestReference featureManifest in featureManifestReferenceList)
            {
                if (featureManifest.Location.Equals(location, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Log.Warning("Multiple features found with the same name '" + featureManifest.Location + "', therefore only the first one found is used.");
                    break;
                }
            }
            return found;
        }


        private List<string> GetElementFiles(FileInfo featureFile, DirectoryInfo childDir)
        {
            List<string> elements = new List<string>();
            FeatureDefinition featureDef = null;
            XmlSerializer xmlSerial = new XmlSerializer(typeof(FeatureDefinition));

            // Always add the feature.xml file to the list
            elements.Add(childDir.Name + @"\" + featureFile.Name);

            using (FileStream fs = featureFile.OpenRead())
            {
                featureDef = (FeatureDefinition)xmlSerial.Deserialize(fs);

                // If no ElementManifest is available, then the feature resource files 
                // are picked up by the ResourceDefinition part of the solution.
                if (featureDef.ElementManifests != null && featureDef.ElementManifests.Items != null)
                {
                    foreach (ElementManifestReference element in featureDef.ElementManifests.Items)
                    {
                        if (!String.IsNullOrEmpty(element.Location))
                        {
                            elements.Add(childDir.Name + @"\" + element.Location);
                        }
                    }
                }
                else
                {
                    // There should normally always be a ElementManifest for a feature,
                    // but it's possible not to use one and only use FeatureReceiver objects.
                    if (string.IsNullOrEmpty(featureDef.ReceiverAssembly) && string.IsNullOrEmpty(featureDef.ReceiverClass))
                    {
                        Log.Warning("No ElementManifest and Feature receiver was found in feature '" + childDir.FullName + "'.");
                    }
                }

                fs.Close();
            }

            return elements;
        }


        /// <summary>
        /// Find and builds the Features going into the wsp file.
        /// </summary>
        /// <param name="parentDir">The feature directory</param>
        /// <returns>FeatureManifestReference[]</returns>
        public FeatureManifestReference[] BuildFeatureManifestReference(DirectoryInfo parentDir, FeatureManifestReference[] featureManifests)
        {
            List<FeatureManifestReference> featureManifestReferenceList = (featureManifests != null) ? new List<FeatureManifestReference>(featureManifests) : new List<FeatureManifestReference>();

            if (IncludeFeatures)
            {
                int pathIndex = parentDir.FullName.Length + 1;
                foreach (DirectoryInfo childDir in FileProvider.GetDirectories(parentDir))
                {
                    string featurePath = childDir.FullName + @"\feature.xml";
                    if (File.Exists(featurePath))
                    {
                        // Only add the feature.xml file, do not add any other file to the manifest.xml
                        FileInfo file = new FileInfo(featurePath);

                        if (FileProvider.IncludeFile(file))
                        {
                            string locationPath = file.FullName.Substring(pathIndex);

                            if (!DoLocationExist(featureManifestReferenceList, locationPath))
                            {

                                FeatureManifestReference feature = new FeatureManifestReference();
                                feature.Location = locationPath;

                                featureManifestReferenceList.Add(feature);

                                Log.Verbose("Feature added: " + feature.Location);

                                // Get the elementfiles
                                ElementFiles.AddRange(GetElementFiles(file, childDir));
                            }
                        }
                    }
                }
            }
            if (featureManifestReferenceList.Count == 0)
            {
                return null;
            }
            return featureManifestReferenceList.ToArray();
        }

        #endregion
    }
}
