Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.SharePoint

Namespace $DefaultNamespace$
    Class $rootname$
        Inherits SPItemEventReceiver
        Public Overloads Overrides Sub ItemAdded(ByVal properties As SPItemEventProperties)
            MyBase.ItemAdded(properties)
        End Sub

        Public Overloads Overrides Sub ItemAdding(ByVal properties As SPItemEventProperties)
            MyBase.ItemAdding(properties)
        End Sub

        Public Overloads Overrides Sub ItemUpdated(ByVal properties As SPItemEventProperties)
            MyBase.ItemUpdated(properties)
        End Sub

        Public Overloads Overrides Sub ItemUpdating(ByVal properties As SPItemEventProperties)
            MyBase.ItemUpdating(properties)
        End Sub

    End Class
End Namespace
