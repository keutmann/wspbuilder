/* Program : WSPBuilderTemplates
 * Created by: Carsten Keutmann
 * Date : 2009
 *  
 * The WSPBuilder comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace WSPBuilderTemplates
{

    public class WorkflowWizard : DefaultFeatureWizard
    {
        public WorkflowWizard()
        {

        }

        public override void AddReferences()
        {
            base.AddReferences();

            // Add a reference to the an assembly
            this.AddReference("System.Workflow.Activities");
            this.AddReference("System.Workflow.Runtime");
            this.AddReference("System.Workflow.ComponentModel");
            this.AddReference("microsoft.office.workflow.tasks");
            this.AddReference("microsoft.sharepoint.WorkflowActions");
        }
    }
}
