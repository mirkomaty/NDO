Imports NDO
Imports System


<NDOPersistent()> _
Public Class MileageAllowance
    Inherits Expense

    ' Properties
    Public Overrides ReadOnly Property Amount() As Decimal
        Get
            Return (CDec(_milesDriven) * New Decimal(4, 0, 0, False, 1))
        End Get
    End Property

    Public Property MilesDriven() As Integer
        Get
            Return _milesDriven
        End Get
        Set(ByVal value As Integer)
            _milesDriven = value
        End Set
    End Property

    Public Overrides ReadOnly Property [Text]() As String
        Get
            Return "Mileage Allowance"
        End Get
    End Property


    ' Fields
    Private _milesDriven As Integer
End Class



