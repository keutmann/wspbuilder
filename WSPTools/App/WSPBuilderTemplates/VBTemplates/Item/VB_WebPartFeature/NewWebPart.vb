Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Web.UI
Imports System.Web.UI.WebControls.WebParts

Namespace $DefaultNamespace$
    <Guid("$guid1$")> _
    Public Class $rootname$
        Inherits Microsoft.SharePoint.WebPartPages.WebPart
        Private _error As Boolean = False
        Private _myProperty As String = Nothing


        <Personalizable(PersonalizationScope.Shared)> _
        <WebBrowsable(True)> _
        <System.ComponentModel.Category("My Property Group")> _
        <WebDisplayName("MyProperty")> _
        <WebDescription("Meaningless Property")> _
        Public Property MyProperty() As String
            Get
                If _myProperty = Nothing Then
                    _myProperty = "Hello SharePoint"
                End If
                Return _myProperty
            End Get
            Set(ByVal value As String)
                _myProperty = value
            End Set
        End Property


        Public Sub New()
            Me.ExportMode = WebPartExportMode.All
        End Sub

        ''' <summary>
        ''' Create all your controls here for rendering.
        ''' Try to avoid using the RenderWebPart() method.
        ''' </summary>
        Protected Overloads Overrides Sub CreateChildControls()
            If Not _error Then
                Try

                    MyBase.CreateChildControls()

                    ' Your code here...
                    Me.Controls.Add(New LiteralControl(Me.MyProperty))
                Catch ex As Exception
                    HandleException(ex)
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Ensures that the CreateChildControls() is called before events.
        ''' Use CreateChildControls() to create your controls.
        ''' </summary>
        ''' <param name="e"></param>
        Protected Overloads Overrides Sub OnLoad(ByVal e As EventArgs)
            If Not _error Then
                Try
                    MyBase.OnLoad(e)

                    ' Your code here...
                    Me.EnsureChildControls()
                Catch ex As Exception
                    HandleException(ex)
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Clear all child controls and add an error message for display.
        ''' </summary>
        ''' <param name="ex"></param>
        Private Sub HandleException(ByVal ex As Exception)
            Me._error = True
            Me.Controls.Clear()
            Me.Controls.Add(New LiteralControl(ex.Message))
        End Sub
    End Class
End Namespace
