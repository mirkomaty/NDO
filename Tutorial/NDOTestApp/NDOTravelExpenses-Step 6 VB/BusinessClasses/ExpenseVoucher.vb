Imports NDO
Imports System


<NDOPersistent()> _
Public Class ExpenseVoucher
    Inherits Expense

    ' Properties
    Public Overrides ReadOnly Property Amount() As Decimal
        Get
            Return _sum
        End Get
    End Property

    Public Property Sum() As Decimal
        Get
            Return _sum
        End Get
        Set(ByVal value As Decimal)
            _sum = value
        End Set
    End Property

    Public Overrides ReadOnly Property [Text]() As String
        Get
            Return _text
        End Get
    End Property

    Public Property VoucherText() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
        End Set
    End Property


    ' Fields
    Private _sum As Decimal
    Private _text As String
End Class



