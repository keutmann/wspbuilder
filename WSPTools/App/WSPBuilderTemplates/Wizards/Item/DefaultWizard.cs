/* Program : WSPBuilderCustomActions
 * Created by: Tom Clarkson
 * Date : 2007
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace WSPBuilderTemplates
{

    /// <summary>
    /// An concrete Wizard class that does nothing beyond what is in the base class. Use for
    /// item templates that require the project property substitutions but no additional functionality.
    /// </summary>
    class DefaultWizard : BaseItemWizard
    {
        public DefaultWizard()
        {

        }
    }
}
