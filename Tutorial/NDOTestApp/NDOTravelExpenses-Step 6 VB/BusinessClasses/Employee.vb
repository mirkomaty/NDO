Imports NDO
Imports System
Imports System.Linq
Imports System.Collections


<NDOPersistent()> _
Public Class Employee
    ' Methods
    Public Sub New()
        _travels = New List(Of Travel)
    End Sub



    ' Properties
    Public Property Address() As Address
        Get
            Return _address
        End Get
        Set(ByVal value As Address)
            _address = value
        End Set
    End Property

    Public Property FirstName() As String
        Get
            Return _firstName
        End Get
        Set(ByVal value As String)
            _firstName = value
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return _lastName
        End Get
        Set(ByVal value As String)
            _lastName = value
        End Set
    End Property

    Public Property Travels() As IEnumerable(Of Travel)
        Get
            Return _travels
        End Get
        Set(ByVal Value As IEnumerable(Of Travel))
            _travels = value.ToList()
        End Set
    End Property
    Public Function NewTravel() As Travel
        Dim t As Travel = New Travel
        _travels.Add(t)
        Return t
    End Function
    Public Sub RemoveTravel(t As Travel)
        If _travels.Contains(t) Then
            _travels.Remove(t)
        End If
    End Sub


    ' Fields
    <NDORelation(RelationInfo.Composite)> _
    Private _address As Address
    Private _firstName As String
    Private _lastName As String
    <NDORelation(GetType(Travel), RelationInfo.Composite)> _
    Private _travels As List(Of Travel)
End Class



