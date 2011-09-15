Imports NDO
Imports System


<NDOPersistent()> _
Public Class PerDiemAllowance
    Inherits Expense

    ' Properties
    Public Overrides ReadOnly Property Amount() As Decimal
        Get
            If (_hours <= New Decimal(8)) Then
                Return New Decimal(0)
            End If
            If (_hours <= New Decimal(10)) Then
                Return New Decimal(5)
            End If
            If (_hours <= New Decimal(12)) Then
                Return New Decimal(10)
            End If
            Return New Decimal(20)
        End Get
    End Property

    Public Property Hours() As Decimal
        Get
            Return _hours
        End Get
        Set(ByVal value As Decimal)
            _hours = value
        End Set
    End Property

    Public Overrides ReadOnly Property [Text]() As String
        Get
            Return "Per Diem Allowance"
        End Get
    End Property


    ' Fields
    Private _hours As Decimal
End Class



