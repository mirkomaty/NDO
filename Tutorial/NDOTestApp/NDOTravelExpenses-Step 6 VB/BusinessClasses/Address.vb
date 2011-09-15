Imports NDO
Imports System


<NDOPersistent()> _
Public Class Address
    ' Properties
    Public Property City() As String
        Get
            Return _city
        End Get
        Set(ByVal value As String)
            _city = value
        End Set
    End Property

    Public Property CountryCode() As String
        Get
            Return _countryCode
        End Get
        Set(ByVal value As String)
            _countryCode = value
        End Set
    End Property

    Public Property Street() As String
        Get
            Return _street
        End Get
        Set(ByVal value As String)
            _street = value
        End Set
    End Property

    Public Property Zip() As String
        Get
            Return _zip
        End Get
        Set(ByVal value As String)
            _zip = value
        End Set
    End Property


    ' Fields
    Private _city As String
    Private _countryCode As String
    Private _street As String
    Private _zip As String
End Class



