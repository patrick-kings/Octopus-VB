Imports System.Web.Mvc

Namespace Controllers
    Public Class InitiateController

        Private apikey As String = ""
        Private userName As String = ""
        Private AT_virtualNumber As String = ""

        Dim gateway As New AfricasTalkingGateway(userName, apikey)

        '    
        ' SMS function
        '

        Public Sub sendMessage(phoneNumber, message)

            gateway.sendMessage(phoneNumber, message)

        End Sub

        '
        ' Make call function
        '

        Public Sub makeCall(phoneNumber)

            gateway.call(AT_virtualNumber, phoneNumber)

        End Sub

    End Class
End Namespace