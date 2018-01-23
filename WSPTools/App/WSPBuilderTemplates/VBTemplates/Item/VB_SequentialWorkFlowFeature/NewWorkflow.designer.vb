Imports System
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Collections
Imports System.Drawing
Imports System.Reflection
Imports System.Workflow.ComponentModel.Compiler
Imports System.Workflow.ComponentModel.Serialization
Imports System.Workflow.ComponentModel
Imports System.Workflow.ComponentModel.Design
Imports System.Workflow.Runtime
Imports System.Workflow.Activities
Imports System.Workflow.Activities.Rules

Namespace $DefaultNamespace$
	Partial Class $rootname$
		#region "Designer generated code"

		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.CanModifyActivities = True
			Dim correlationtoken1 As New System.Workflow.Runtime.CorrelationToken()
			Dim activitybind1 As New System.Workflow.ComponentModel.ActivityBind()
			Dim activitybind2 As New System.Workflow.ComponentModel.ActivityBind()
			Me.onWorkflowActivated1 = New Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated()
			' 
			' onWorkflowActivated1
			' 
			correlationtoken1.Name = "workflowToken"
			correlationtoken1.OwnerActivityName = "$rootname$"
			Me.onWorkflowActivated1.CorrelationToken = correlationtoken1
			Me.onWorkflowActivated1.EventName = "OnWorkflowActivated"
			Me.onWorkflowActivated1.Name = "onWorkflowActivated1"
			activitybind1.Name = "$rootname$"
			activitybind1.Path = "workflowId"
			activitybind2.Name = "$rootname$"
			activitybind2.Path = "workflowProperties"
			Me.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, (DirectCast((activitybind1), System.Workflow.ComponentModel.ActivityBind)))
			Me.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, (DirectCast((activitybind2), System.Workflow.ComponentModel.ActivityBind)))
			' 
			' $rootname$
			' 
			Me.Activities.Add(Me.onWorkflowActivated1)
			Me.Name = "$rootname$"
			Me.CanModifyActivities = False

		End Sub
#End Region

		Private onWorkflowActivated1 As Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated


	End Class
End Namespace

