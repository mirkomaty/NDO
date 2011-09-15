Imports NDO
Imports System
Imports System.Collections


<NDOPersistent()> _
Public Class Employee
    ' Methods
    Public Sub New()
        _travels = New ArrayList
    End Sub

    Public Function NewTravel() As Travel
        Dim travel1 As New Travel
        _travels.Add(travel1)
        Return travel1
    End Function

    Public Sub RemoveTravel(ByVal t As Travel)
        If _travels.Contains(t) Then
            _travels.Remove(t)
        End If
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

    Public Property Travels() As IList
        Get
            Return ArrayList.ReadOnly(_travels)
        End Get
        Set(ByVal value As IList)
            _travels = value
        End Set
    End Property


    ' Fields
    <NDORelation(RelationInfo.Composite)> _
    Private _address As Address
    Private _firstName As String
    Private _lastName As String
    <NDORelation(GetType(Travel), RelationInfo.Composite)> _
    Private _travels As IList
End Class



