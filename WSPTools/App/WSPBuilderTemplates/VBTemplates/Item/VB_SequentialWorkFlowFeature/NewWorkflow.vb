Imports System
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Collections
Imports System.Drawing
Imports System.Workflow.ComponentModel.Compiler
Imports System.Workflow.ComponentModel.Serialization
Imports System.Workflow.ComponentModel
Imports System.Workflow.ComponentModel.Design
Imports System.Workflow.Runtime
Imports System.Workflow.Activities
Imports System.Workflow.Activities.Rules
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Workflow
Imports Microsoft.SharePoint.WorkflowActions
Imports Microsoft.Office.Workflow.Utility

Namespace $DefaultNamespace$
    Partial Public NotInheritable Class $rootname$
        Inherits StateMachineWorkflowActivity
        Public Sub New()
            InitializeComponent()
        End Sub
        Public workflowId As Guid = Nothing
        Public workflowProperties As SPWorkflowActivationProperties = New Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties()
    End Class

End Namespace

