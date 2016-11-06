Imports System.Web.Mvc

Namespace Controllers
    Public Class InitiateController
        '  Inherits Controller

        Private apikey As String = ""
        Private userName As String = ""
        Private AT_virtualNumber As String = ""
        
        '    
        ' SMS function
        '

        Public Sub sendMessage(phoneNumber, message)

            Dim gateway As New AfricasTalkingGateway(userName, apikey)

            gateway.sendMessage(phoneNumber, message)

        End Sub
        '
        ' Make call function
        '
        Public Sub makeCall(phoneNumber)

            Dim gateway As New AfricasTalkingGateway(userName, apikey)

            gateway.call(AT_virtualNumber, phoneNumber)

        End Sub

    End Class
End Namespace