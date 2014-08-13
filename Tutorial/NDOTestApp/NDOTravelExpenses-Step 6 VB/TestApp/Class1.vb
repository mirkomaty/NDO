Imports BusinessClasses
Imports NDO
Imports System
Imports System.Linq
Imports System.Drawing

Namespace TestApp
    Friend Class Class1

        ' Methods
        Public Shared Sub Main(ByVal args As String())
            CheckBits()

            Class1.ShowTheUseoftheFactory()
            Class1.CreateObjects()
            Class1.QueryObjects()
        End Sub


        Private Shared Sub CreateObjects()
            ' Expenses sample
            Dim pm As New PersistenceManager
            pm.BuildDatabase()
            Dim employee1 As New Employee
            employee1.FirstName = "John"
            employee1.LastName = "Becker"
            pm.MakePersistent(employee1)
            Dim travel1 As Travel = employee1.NewTravel
            travel1.Purpose = "NDO Workshop"
            Dim voucher1 As New ExpenseVoucher
            voucher1.VoucherText = "Taxi"
            voucher1.Sum = New Decimal(30)
            voucher1.Date = DateTime.Now.Date
            travel1.AddExpense(voucher1)
            Dim allowance1 As New MileageAllowance
            allowance1.MilesDriven = 200
            allowance1.Date = DateTime.Now.Date
            travel1.AddExpense(allowance1)
            Dim allowance2 As New PerDiemAllowance
            allowance2.Hours = New Decimal(125, 0, 0, False, 1)
            allowance2.Date = DateTime.Now.Date
            travel1.AddExpense(allowance2)
            pm.Save()

            ' PictureHeader and Picture sample
            Dim image1 As Image = Image.FromFile("..\..\..\Building.bmp")
            Dim picture1 As New Picture
            picture1.Name = "Our first test picture"
            picture1.Image = image1
            picture1.CreationDate = DateTime.Now
            pm.MakePersistent(picture1)
            pm.Save()

        End Sub

        Private Shared Sub CheckBits()
            Console.WriteLine("Running as " + (IntPtr.Size * 8).ToString() + " bit app.")
        End Sub

        Private Shared Sub QueryObjects()
            Console.WriteLine("-------- Query for expenses ------------")
            Dim pm As New PersistenceManager
            Dim query1 As Query = pm.NewQuery(GetType(Employee))
            Dim employee1 As Employee = DirectCast(query1.ExecuteSingle, Employee)
            Dim travel1 As Travel = employee1.Travels.FirstOrDefault ' Linq
            Console.WriteLine(("Costs of the travel with the purpose " & travel1.Purpose & ":"))
            Dim expense1 As Expense
            For Each expense1 In travel1.Expenses
                Console.WriteLine(String.Concat(New String() {expense1.Date.ToShortDateString, " ", expense1.Text, " ", expense1.Amount.ToString}))
            Next

            Console.WriteLine("-------- Query for pictures ------------")
            query1 = pm.NewQuery(GetType(PictureHeader))
            Dim l As IList = query1.Execute()
            ' In a real world application we'd show the headers in a UI
            Dim ph As PictureHeader
            For Each ph In l
                Console.WriteLine(ph.Name + " " + ph.CreationDate.ToShortDateString)
            Next
            ' Suppose we selected the first PictureHeader with the index 0
            Dim pc As IPersistenceCapable = DirectCast(l(0), IPersistenceCapable)
            ' Now we convert the object id in an object id for the type Picture
            ' and get the Picture object
            Dim p As Picture = DirectCast(pm.FindObject(GetType(Picture), pc.NDOObjectId.Id.Value), Picture)
            Dim image1 As Image = p.Image
            Console.WriteLine("Picture: " & p.Name & ": " & image1.Height & "x" & image1.Width)
        End Sub

        Private Shared Sub ShowTheUseoftheFactory()
            Console.WriteLine("-------- Show the use of the Factory ------------")
            Dim employee1 As New Employee
            employee1.FirstName = "John"
            employee1.LastName = "Becker"
            Dim travel1 As Travel = employee1.NewTravel
            travel1.Purpose = "NDO Workshop"
            Dim factory1 As New ExpenseFactory
            ' Suppose we have selected the first entry of the Expense type list in the UI.
            ' The first entry has the index 0.
            Dim expense1 As Expense = factory1.NewExpense(factory1.Types(0))
            Console.WriteLine(("The new Expense has the type: " & expense1.GetType.FullName))
        End Sub

    End Class
End Namespace


