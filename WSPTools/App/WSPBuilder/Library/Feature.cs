using System;
using System.Collections.Generic;
using System.Text;

namespace Keutmann.SharePoint.WSPBuilder.Library
{
    public class Feature
    {
        private string _featureName;
        private Guid _featureID;

        public Feature(Guid featureID, string featureName)
        {
            _featureID = featureID;
            _featureName = featureName;
        }

        public Guid ID
        {
            get
            {
                return _featureID;
            }
        }

        public string Name
        {
            get
            {
                return _featureName;
            }
        }
    }
}
