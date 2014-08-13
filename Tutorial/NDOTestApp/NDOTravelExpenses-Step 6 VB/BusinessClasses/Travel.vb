Imports NDO
Imports System
Imports System.Linq
Imports System.Collections


<NDOPersistent()> _
Public Class Travel
    ' Methods
    Public Sub New()
        _expenses = New List(Of Expense)
        _countries = New List(Of Country)
    End Sub


    Public Property Employee() As Employee
        Get
            Return _employee
        End Get
        Set(ByVal value As Employee)
            _employee = value
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
    <NDORelation> _
    Private _countries As IList(Of Country)
    <NDORelation()> _
    Private _employee As Employee
    <NDORelation(RelationInfo.Composite)> _
    Private _expenses As IList(Of Expense)

    ' Accessor Properties

    Public Property Expenses() As IEnumerable(Of Expense)
        Get
            Return _expenses
        End Get
        Set(ByVal Value As IEnumerable(Of Expense))
            _expenses = Value.ToList()
        End Set
    End Property
    Public Sub RemoveExpense(e As Expense)
        If _expenses.Contains(e) Then
            _expenses.Remove(e)
        End If
    End Sub
    Public Sub AddExpense(ByVal ex As Expense)
        _expenses.Add(ex)
    End Sub

    Private _purpose As String

    Public Property Countries() As IEnumerable(Of Country)
        Get
            Return _countries
        End Get
        Set(ByVal Value As IEnumerable(Of Country))
            _countries = Value.ToList()
        End Set
    End Property
    Public Sub AddCountry(c As Country)
        _countries.Add(c)
    End Sub
    Public Sub RemoveCountry(c As Country)
        If _countries.Contains(c) Then
            _countries.Remove(c)
        End If
    End Sub

End Class



