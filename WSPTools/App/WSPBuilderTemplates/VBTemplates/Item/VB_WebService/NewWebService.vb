Imports System
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols


Namespace $DefaultNamespace$
    <WebService([Namespace]:="http://tempuri.org/")> _
    <WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    Public Class $rootname$
        Inherits System.Web.Services.WebService

        <WebMethod()> _
        Public Function HelloWorld() As String
            Return "Hello World"
        End Function

    End Class

End Namespace


