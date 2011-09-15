Imports NDO
Imports System
Imports System.Collections


<NDOPersistent()> _
Public Class Country
    ' Methods
    Public Sub New()
        _travels = New ArrayList
    End Sub

    Public Sub AddTravel(ByVal t As Travel)
        _travels.Add(t)
    End Sub

    Public Sub RemoveTravel(ByVal t As Travel)
        If _travels.Contains(t) Then
            _travels.Remove(t)
        End If
    End Sub


    ' Properties
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
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
    Private _name As String
    <NDORelation(GetType(Travel))> _
    Private _travels As IList
End Class



