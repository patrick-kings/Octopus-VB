Imports System.IO
Imports System.Net
Imports System.Web.Script.Serialization

'
' AfricasTalkingGatewayException class
'
Public Class AfricasTalkingGatewayException
    Inherits Exception
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub
End Class


'
' AfricasTalkingGateway class
'
Public Class AfricasTalkingGateway
    Private _username As String
    Private _apiKey As String
    Private responseCode As Integer
    Private serializer As JavaScriptSerializer

    Private Shared SMS_URLString As String = "https://api.africastalking.com/version1/messaging"
    Private Shared VOICE_URLString As String = "https://voice.africastalking.com"
    Private Shared USERDATA_URLString As String = "https://api.africastalking.com/version1/user"
    Private Shared SUBSCRIPTION_URLString As String = "https://api.africastalking.com/version1/subscription"
    Private Shared AIRTIME_URLString As String = "https://api.africastalking.com/version1/airtime"

    ' Change this flag to true to view full server response
    Private Shared DEBUG As Boolean = False

    Public Sub New(username_ As String, apiKey_ As String)
        _username = username_
        _apiKey = apiKey_
        serializer = New JavaScriptSerializer()
    End Sub


    'Messaging functions
    Public Function sendMessage(to_ As String, message_ As String, Optional from_ As String = Nothing, Optional bulkSMSMode_ As Integer = 1, Optional options_ As Hashtable = Nothing) As Object()
        Dim data As New Hashtable()
        data("username") = _username
        data("to") = to_
        data("message") = message_

        If from_ IsNot Nothing Then
            data("from") = from_
            data("bulkSMSMode") = Convert.ToString(bulkSMSMode_)

            If options_ IsNot Nothing Then
                If options_.Contains("keyword") Then
                    data("keyword") = options_("keyword")
                End If

                If options_.Contains("linkId") Then
                    data("linkId") = options_("linkId")
                End If

                If options_.Contains("enqueue") Then
                    data("enqueue") = options_("enqueue")
                End If

                If options_.Contains("retryDurationInHours") Then
                    data("retryDurationInHours") = options_("retryDurationInHours")
                End If
            End If
        End If

        Dim response As String = sendPostRequest(data, SMS_URLString)

        If responseCode = HttpStatusCode.Created Then
            Dim json As Object = serializer.DeserializeObject(response)
            Dim smsMessageData As Object = json("SMSMessageData")

            Dim recipients As Object() = TryCast(smsMessageData("Recipients"), Object())
            If recipients.Length > 0 Then
                Return recipients
            End If

            Throw New AfricasTalkingGatewayException(TryCast(smsMessageData("Message"), String))
        End If

        Throw New AfricasTalkingGatewayException(response)
    End Function

    Public Function fetchMessages(lastReceivedId_ As Integer) As Object()
        Dim data As New Hashtable()
        Dim url As String = SMS_URLString + "?username=" + _username + "&lastReceivedId=" + Convert.ToString(lastReceivedId_)
        Dim response As String = sendGetRequest(url)
        If responseCode = HttpStatusCode.OK Then
            Dim json As Object = serializer.DeserializeObject(response)
            Dim smsMessageData As Object = json("SMSMessageData")
            Dim data1 As Object() = TryCast(smsMessageData("Messages"), Object())
            Return data1
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function


    'Subscription functions
    Public Function createSubcsription(phoneNumber_ As String, shortCode_ As String, keyword_ As String) As Object
        If phoneNumber_.Length = 0 Or shortCode_.Length = 0 Or keyword_.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If

        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumber") = phoneNumber_
        data("shortCode") = shortCode_
        data("keyword") = keyword_

        Dim urlString As String = SUBSCRIPTION_URLString + "/create"

        Dim response As String = sendPostRequest(data, urlString)
        If responseCode = HttpStatusCode.Created Then
            Dim result As Object = serializer.DeserializeObject(response)
            Return result
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function


    Public Function deleteSubscription(phoneNumber_ As String, shortCode_ As String, keyword_ As String) As Object
        If phoneNumber_.Length = 0 Or shortCode_.Length = 0 Or keyword_.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply phone number, short code and keyword")
        End If

        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumber") = phoneNumber_
        data("shortCode") = shortCode_
        data("keyword") = keyword_

        Dim urlString As String = SUBSCRIPTION_URLString + "/delete"

        Dim response As String = sendPostRequest(data, urlString)

        If responseCode = HttpStatusCode.Created Then
            Dim json As Object = serializer.DeserializeObject(response)
            Return json
        End If

        Throw New AfricasTalkingGatewayException(response)
    End Function


    Public Function fetchPremiumSubcriptions(shortCode_ As String, keyword_ As String, Optional lastReceivedId_ As Integer = 0) As Object()
        If shortCode_.Length = 0 Or keyword_.Length = 0 Then
            Throw New AfricasTalkingGatewayException("Please supply short code and keyword")
        End If
        Dim urlString As String = SUBSCRIPTION_URLString + "?username=" + _username + "&shortCode=" + shortCode_ + "&keyword=" + keyword_ + "&lastReceivedId=" + Convert.ToString(lastReceivedId_)

        Dim response As String = sendGetRequest(urlString)

        If responseCode = HttpStatusCode.OK Then
            Dim json As Object = serializer.DeserializeObject(response)
            Dim data As Object() = TryCast(json("responses"), Object())
            Return data
        End If

        Throw New AfricasTalkingGatewayException(response)
    End Function


    'Voice functions
    Public Function [call](from_ As String, to_ As String) As Object()
        Dim data As New Hashtable()
        data("username") = _username
        data("from") = from_
        data("to") = to_

        Dim urlString As String = VOICE_URLString + "/call"
        Dim response As String = sendPostRequest(data, urlString)

        Dim jsObject As Object = serializer.DeserializeObject(response)

        If responseCode = HttpStatusCode.OK Or responseCode = HttpStatusCode.Created Then
            If jsObject("errorMessage") <> "None" Then
                Return TryCast(jsObject("entries"), Object())
            End If

            Throw New AfricasTalkingGatewayException(jsObject("errorMessage"))
        End If

        Throw New AfricasTalkingGatewayException(response)

    End Function


    Public Function getNumQueuedCalls(phoneNumber_ As String, Optional queueName_ As String = Nothing) As Object()
        Dim data As New Hashtable()
        data("username") = _username
        data("phoneNumbers") = phoneNumber_

        If queueName_ IsNot Nothing Then
            data("queueName") = queueName_
        End If

        Dim urlString As String = VOICE_URLString + "/queueStatus"

        Dim response As String = sendPostRequest(data, urlString)
        Dim jsObject As Object = serializer.DeserializeObject(response)

        If responseCode = HttpStatusCode.OK Or responseCode = HttpStatusCode.Created Then
            If jsObject("errorMessage") <> "None" Then
                Return TryCast(jsObject("entries"), Object())
            End If

            Throw New AfricasTalkingGatewayException(jsObject("errorMessage"))
        End If

        Throw New AfricasTalkingGatewayException(response)

    End Function


    Public Sub uploadMediaFile(urlString_ As String)
        Dim data As New Hashtable()
        data("username") = _username
        data("url") = urlString_
        Dim url As String = VOICE_URLString + "/mediaUpload"

        Dim response As String = sendPostRequest(data, url)

        If responseCode = HttpStatusCode.OK Or responseCode = HttpStatusCode.Created Then
            Dim jsObject As Object = serializer.DeserializeObject(response)

            If jsObject("errorMessage") <> "None" Then
                Throw New AfricasTalkingGatewayException(jsObject("errorMessage"))
            End If
        End If

        Throw New AfricasTalkingGatewayException(response)
    End Sub


    'Airtime function
    Public Function sendAirtime(recipients_ As List(Of Hashtable)) As Object()
        Dim recipientString As String = serializer.Serialize(recipients_)
        Dim data As New Hashtable()
        data("username") = _username
        data("recipients") = recipientString
        Dim urlString As String = AIRTIME_URLString + "/send"

        Dim response As String = sendPostRequest(data, urlString)

        If responseCode = HttpStatusCode.Created Then
            Dim decoded As Object = serializer.DeserializeObject(response)
            Dim result As Object() = TryCast(decoded("responses"), Object())
            If result.Length > 0 Then
                Return result
            End If
            Throw New AfricasTalkingGatewayException(decoded("errorMessage"))
        End If
        Throw New AfricasTalkingGatewayException(response)
    End Function


    'userdata function
    Public Function getUserData() As Object
        Dim urlString As String = USERDATA_URLString + "?username=" + _username

        Dim response As String = sendGetRequest(urlString)

        If responseCode = HttpStatusCode.OK Then
            Dim json As Object = serializer.DeserializeObject(response)
            Dim data As Object = json("UserData")
            Return data
        End If

        Throw New AfricasTalkingGatewayException(response)
    End Function


    'HTTP request methods
    Private Function sendPostRequest(dataMap_ As Hashtable, urlString_ As String) As String
        Try
            Dim dataStr As String = ""
            For Each key As String In dataMap_.Keys
                If dataStr.Length > 0 Then
                    dataStr += "&"
                End If
                Dim value As String = DirectCast(dataMap_(key), String)
                dataStr += HttpUtility.UrlEncode(key, Encoding.UTF8)
                dataStr += "=" + HttpUtility.UrlEncode(value, Encoding.UTF8)
            Next

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(dataStr)

            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf RemoteCertificateValidationCallback
            Dim webRequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(urlString_), HttpWebRequest)

            webRequest__1.Method = "POST"
            webRequest__1.ContentType = "application/x-www-form-urlencoded"
            webRequest__1.ContentLength = byteArray.Length
            webRequest__1.Accept = "application/json"

            webRequest__1.Headers.Add("apiKey", _apiKey)

            Dim webpageStream As Stream = webRequest__1.GetRequestStream()
            webpageStream.Write(byteArray, 0, byteArray.Length)
            webpageStream.Close()

            Dim httpResponse As HttpWebResponse = DirectCast(webRequest__1.GetResponse(), HttpWebResponse)

            responseCode = httpResponse.StatusCode()

            Dim webpageReader As New StreamReader(httpResponse.GetResponseStream())
            Dim responseString As String = webpageReader.ReadToEnd()

            If DEBUG Then
                System.Console.WriteLine("Full response: " + responseString)
            End If

            Return responseString

        Catch ex As WebException
            If ex.Response Is Nothing Then
                Throw New AfricasTalkingGatewayException(ex.Message)
            End If
            Dim reader As StreamReader = New StreamReader(ex.Response.GetResponseStream())

            Dim responseString As String = reader.ReadToEnd()

            If DEBUG Then
                System.Console.WriteLine("Full response: " + responseString)
            End If

            Return responseString

        Catch ex As AfricasTalkingGatewayException
            Throw New AfricasTalkingGatewayException(ex.Message())
        End Try
    End Function


    Private Function sendGetRequest(urlString_ As String) As String
        Try
            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf RemoteCertificateValidationCallback
            Dim webRequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(urlString_), HttpWebRequest)

            webRequest__1.Method = "GET"
            webRequest__1.Accept = "application/json"

            webRequest__1.Headers.Add("apikey", _apiKey)

            Dim httpResponse As HttpWebResponse = DirectCast(webRequest__1.GetResponse(), HttpWebResponse)

            responseCode = httpResponse.StatusCode()

            Dim webpageReader As New StreamReader(httpResponse.GetResponseStream())
            Dim responseString As String = webpageReader.ReadToEnd()

            If DEBUG Then
                System.Console.WriteLine("Full response: " + responseString)
            End If

            Return responseString

        Catch ex As WebException
            If ex.Response Is Nothing Then
                Throw New AfricasTalkingGatewayException(ex.Message)
            End If
            Dim reader As StreamReader = New StreamReader(ex.Response.GetResponseStream())

            Dim responseString As String = reader.ReadToEnd()

            If DEBUG Then
                System.Console.WriteLine("Full response: " + responseString)
            End If

            Return responseString

        Catch ex As AfricasTalkingGatewayException
            Throw New AfricasTalkingGatewayException(ex.Message())
        End Try
    End Function


    Private Function RemoteCertificateValidationCallback(_sender As Object, _certificate As System.Security.Cryptography.X509Certificates.X509Certificate, _chain As System.Security.Cryptography.X509Certificates.X509Chain, _sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
End Class
