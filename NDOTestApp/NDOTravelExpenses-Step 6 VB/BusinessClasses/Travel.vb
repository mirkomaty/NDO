Imports NDO
Imports System
Imports System.Collections


<NDOPersistent()> _
Public Class Travel
    ' Methods
    Public Sub New()
        _expenses = New ArrayList
        _countries = New ArrayList
    End Sub

    Public Sub AddCountry(ByVal c As Country)
        _countries.Add(c)
    End Sub

    Public Sub AddExpense(ByVal ex As Expense)
        _expenses.Add(ex)
    End Sub

    Public Sub RemoveCountry(ByVal c As Country)
        _countries.Remove(c)
    End Sub

    Public Sub RemoveExpense(ByVal ex As Expense)
        If _expenses.Contains(ex) Then
            _expenses.Remove(ex)
        End If
    End Sub


    ' Properties
    Public Property Countries() As IList
        Get
            Return ArrayList.ReadOnly(_countries)
        End Get
        Set(ByVal value As IList)
            _countries = value
        End Set
    End Property

    Public Property Employee() As Employee
        Get
            Return _employee
        End Get
        Set(ByVal value As Employee)
            _employee = value
        End Set
    End Property

    Public Property Expenses() As IList
        Get
            Return ArrayList.ReadOnly(_expenses)
        End Get
        Set(ByVal value As IList)
            _expenses = value
        End Set
    End Property

    Public Property Purpose() As String
        Get
            Return _purpose
        End Get
        Set(ByVal value As String)
            _purpose = value
        End Set
    End Property


    ' Fields
    <NDORelation(GetType(Country))> _
    Private _countries As IList
    <NDORelation()> _
    Private _employee As Employee
    <NDORelation(GetType(Expense), RelationInfo.Composite)> _
    Private _expenses As IList
    Private _purpose As String
End Class



