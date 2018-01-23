Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports System.Web.UI
Imports System.Web.UI.WebControls


Namespace DefaultNamespace
    Public Class $rootname$
        Inherits SPFieldText
        Private Shared CustomPropertyNames As String() = New String() {"MyCustomProperty"}

        Public Sub New(ByVal fields As SPFieldCollection, ByVal fieldName As String)
            MyBase.New(fields, fieldName)
            InitProperties()
        End Sub

        Public Sub New(ByVal fields As SPFieldCollection, ByVal typeName As String, ByVal displayName As String)
            MyBase.New(fields, typeName, displayName)
            InitProperties()
        End Sub

#Region "Property storage and bug workarounds - do not edit"

        ''' <summary>
        ''' Indicates that the field is being created rather than edited. This is necessary to 
        ''' work around some bugs in field creation.
        ''' </summary>
        Public Property IsNew() As Boolean
            Get
                Return _IsNew
            End Get
            Set(ByVal value As Boolean)
                _IsNew = value
            End Set
        End Property
        Private _IsNew As Boolean = False

        ''' <summary>
        ''' Backing fields for custom properties. Using a dictionary to make it easier to abstract
        ''' details of working around SharePoint bugs.
        ''' </summary>
        Private CustomProperties As New Dictionary(Of String, String)()

        ''' <summary>
        ''' Static store to transfer custom properties between instances. This is needed to allow
        ''' correct saving of custom properties when a field is created - the custom property 
        ''' implementation is not used by any out of box SharePoint features so is really buggy.
        ''' </summary>
        Private Shared CustomPropertiesForNewFields As New Dictionary(Of String, String)()

        ''' <summary>
        ''' Initialise backing fields from base property store
        ''' </summary>
        Private Sub InitProperties()
            For Each propertyName As String In CustomPropertyNames
                CustomProperties(propertyName) = MyBase.GetCustomProperty(propertyName) + ""
            Next
        End Sub

        ''' <summary>
        ''' Take properties from either the backing fields or the static store and 
        ''' put them in the base property store
        ''' </summary>
        Private Sub SaveProperties()
            For Each propertyName As String In CustomPropertyNames
                MyBase.SetCustomProperty(propertyName, GetCustomProperty(propertyName))
            Next
        End Sub

        ''' <summary>
        ''' Get an identifier for the field being added/edited that will be unique even if
        ''' another user is editing a property of the same name.
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        Private Function GetCacheKey(ByVal propertyName As String) As String
			Return SPContext.Current.GetHashCode() + "_" + (If(ParentList = Nothing, "SITE", ParentList.ID.ToString())) + "_" + propertyName
        End Function

        ''' <summary>
        ''' Replace the buggy base implementation of SetCustomProperty
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <param name="propertyValue"></param>
        Public Shadows Sub SetCustomProperty(ByVal propertyName As String, ByVal propertyValue As Object)
            If IsNew Then
                ' field is being added - need to put property in cache
                CustomPropertiesForNewFields(GetCacheKey(propertyName)) = propertyValue + ""
            End If

            CustomProperties(propertyName) = propertyValue + ""
        End Sub

        ''' <summary>
        ''' Replace the buggy base implementation of GetCustomProperty
        ''' </summary>
        ''' <param name="propertyName"></param>
        ''' <param name="propertyValue"></param>
        Public Shadows Function GetCustomProperty(ByVal propertyName As String) As Object
            If Not IsNew AndAlso CustomPropertiesForNewFields.ContainsKey(GetCacheKey(propertyName)) Then
                Dim s As String = CustomPropertiesForNewFields(GetCacheKey(propertyName))
                CustomPropertiesForNewFields.Remove(GetCacheKey(propertyName))
                CustomProperties(propertyName) = s
                Return s
            Else
                Return CustomProperties(propertyName)
            End If
        End Function

        ''' <summary>
        ''' Called when a field is created. Without this, update is not called and custom properties
        ''' are not saved.
        ''' </summary>
        ''' <param name="op"></param>
        Public Overloads Overrides Sub OnAdded(ByVal op As SPAddFieldOptions)
            MyBase.OnAdded(op)
            Update()
        End Sub
#End Region


        Public Overloads Overrides ReadOnly Property FieldRenderingControl() As BaseFieldControl
            Get
                Dim fieldControl As BaseFieldControl = New $rootname$Control(Me)

                fieldControl.FieldName = InternalName

                Return fieldControl
            End Get
        End Property

        Public Overloads Overrides Sub Update()
            SaveProperties()
            MyBase.Update()
        End Sub

        Public Property MyCustomProperty() As String
            Get
                Return Me.GetCustomProperty("MyCustomProperty") + ""
            End Get
            Set(ByVal value As String)
                Me.SetCustomProperty("MyCustomProperty", value)
            End Set
        End Property



    End Class

End Namespace

