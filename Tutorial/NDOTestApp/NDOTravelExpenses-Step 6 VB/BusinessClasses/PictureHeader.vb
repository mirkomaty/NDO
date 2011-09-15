Imports NDO
Imports System


<NDOPersistent()> _
Public Class PictureHeader
    ' Properties
    Public Property CreationDate() As DateTime
        Get
            Return _creationDate
        End Get
        Set(ByVal value As DateTime)
            _creationDate = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property


    ' Fields
    Private _creationDate As DateTime
    Private _name As String
End Class



