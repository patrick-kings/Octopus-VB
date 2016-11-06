Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports Octopus_VB.Models

Namespace Controllers

    <RoutePrefix("api/mobile")>  ' specify the route prefix so that your route can look like...  localhost:8080/api/mobile...
    Public Class MobileController
        Inherits ApiController 'the controller should inherit from the ApiController class

        Private message As String = "Thank you for consuming AfricsTalking API"

        '
        ' initiate communication with the user
        '     

        <Route("ussd")>         ' specify the actual route, your url will now look like... localhost:8080/api/mobile/ussd...
        <HttpPost, ActionName("ussd")> ' state that the method you intend to create is a POST 
        Public Function ussd(<FromBody> ussdResponse As UssdResponse) As HttpResponseMessage ' declare a complex type as input parameter

            Dim httpResponseMessage As HttpResponseMessage
            Dim response As String

            Dim initiateControllerr As New InitiateController

            ' the server may return text parameter as null and hence the need to initialize it to an empty string
            If ussdResponse.text Is Nothing Then
                ussdResponse.text = ""
            End If

            ' loop through the server's text value to determine the next cause of action
            If ussdResponse.text.Equals("", StringComparison.Ordinal) Then
                response =
                    "CON Welcome to AfricasTalking \n" +
                    "1. Receive an inspiration message through  an SMS \n" +
                    "2. Receive an inspiration message through  a Voice call \n" +
                    "3. Know Your phone number"
            ElseIf ussdResponse.text.Equals("1", StringComparison.Ordinal) Then
                initiateControllerr.sendMessage(ussdResponse.phoneNumber, message)
                response = "END You will receive an SMS from AT shortly"

            ElseIf ussdResponse.text.Equals("2", StringComparison.Ordinal) Then
                initiateControllerr.makeCall(ussdResponse.phoneNumber)
                response = "END You will receive a voice call from AT shortly"

            ElseIf ussdResponse.text.Equals("3", StringComparison.Ordinal) Then
                response = "END Your phone number is: " + ussdResponse.phoneNumber
            Else
                response = "END invalid option"
            End If

            httpResponseMessage = Request.CreateResponse(HttpStatusCode.Created, response)

            httpResponseMessage.Content = New StringContent(response, Encoding.UTF8, "text/plain")

            Return httpResponseMessage

        End Function

        '
        ' handle the voice call
        '

        <Route("voice")>
        <HttpPost, ActionName("voice")>
        Public Function voice(<FromBody> HandleCall As HandleCall) As HttpResponseMessage
            Dim httpResponseMessage As HttpResponseMessage
            Dim response As String

            If HandleCall.isActive Is Nothing Then
                HandleCall.isActive = ""
            End If

            If HandleCall.isActive.Equals("1", StringComparison.Ordinal) Then

                response = "<?xml version='1.0' encoding='UTF-8'?><Response><Say>" + message + "</Say></Response>"

                httpResponseMessage = Request.CreateResponse(HttpStatusCode.Created, response)
                httpResponseMessage.Content = New StringContent(response, Encoding.UTF8, "text/plain")

                Return httpResponseMessage
            Else
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK)
                Return httpResponseMessage
            End If

        End Function


    End Class
End Namespace
