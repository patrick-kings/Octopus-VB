Namespace Models

    Public Class UssdResponse
        Private text_ As String
        Private phoneNumber_ As String
        Private sessionId_ As String
        Private serviceCode_ As String

        Public Property text() As String
            Get
                Return text_
            End Get
            Set(value As String)
                text_ = value
            End Set
        End Property

        Public Property phoneNumber() As String
            Get
                Return phoneNumber_
            End Get
            Set(value As String)
                phoneNumber_ = value
            End Set
        End Property

        Public Property sessioId() As String
            Get
                Return sessionId_
            End Get
            Set(value As String)
                sessionId_ = value
            End Set
        End Property

        Public Property serviceCode() As String
            Get
                Return serviceCode_
            End Get
            Set(value As String)
                serviceCode_ = value
            End Set
        End Property
    End Class

    Public Class HandleCall

        Private sessioId_ As String
        Private isActive_ As String
        Private durationInSeconds_ As String
        Private currencyCode_ As String
        Private amount_ As String

        Public Property sessionId() As String
            Get
                Return sessioId_
            End Get
            Set(value As String)
                sessioId_ = value
            End Set
        End Property

        Public Property isActive() As String
            Get
                Return isActive_
            End Get
            Set(value As String)
                isActive_ = value
            End Set
        End Property

        Public Property durationInSeconds() As String
            Get
                Return durationInSeconds_
            End Get
            Set(value As String)
                durationInSeconds_ = value
            End Set
        End Property

        Public Property currencyCode() As String
            Get
                Return currencyCode_
            End Get
            Set(value As String)
                currencyCode_ = value
            End Set
        End Property

        Public Property amount() As String
            Get
                Return amount_
            End Get
            Set(value As String)
                amount_ = value
            End Set
        End Property

    End Class



End Namespace