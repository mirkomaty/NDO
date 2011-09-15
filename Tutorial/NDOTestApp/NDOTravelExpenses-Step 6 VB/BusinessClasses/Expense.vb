Imports NDO
Imports System


<NDOPersistent()> _
Public MustInherit Class Expense
    ' Properties
    Public MustOverride ReadOnly Property Amount() As Decimal

    Public Property [Date]() As DateTime
        Get
            Return _date
        End Get
        Set(ByVal value As DateTime)
            _date = value
        End Set
    End Property

    Public MustOverride ReadOnly Property Text() As String
    ' Fields
    Private _date As DateTime
End Class



