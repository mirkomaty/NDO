Imports System


Public Class ExpenseFactory
    ' Methods
    Public Sub New()
        _theTypes = New String() {"Expense Voucher", "Milage Allowance", "Per Diem Allowance"}
    End Sub

    Public Function NewExpense(ByVal type As String) As Expense
        Select Case type
            Case "Expense Voucher"
                Return New ExpenseVoucher
            Case "Milage Allowance"
                Return New MileageAllowance
            Case "Per Diem Allowance"
                Return New PerDiemAllowance
        End Select
        Throw New Exception(String.Format("Unknown Expense Type: {0}", type))
    End Function


    ' Properties
    Public ReadOnly Property Types() As String()
        Get
            Return _theTypes
        End Get
    End Property


    ' Fields
    Private _theTypes As String()
End Class



