using System;
using System.Linq;
using System.Collections.Generic;
using BusinessClasses;
using NDO;
using System.Drawing;

namespace TestApp
{
    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
			CheckBits();

            ShowTheUseoftheFactory();
            CreateObjects();
            QueryObjects();
        }

        static void ShowTheUseoftheFactory()
        {
            Console.WriteLine("-------- Show the use of the Factory ------------");
            Employee e = new Employee();
            e.FirstName = "John";
            e.LastName = "Becker";
            Travel t = e.NewTravel();
            t.Purpose = "NDO Workshop";
            ExpenseFactory factory = new ExpenseFactory();
            Console.WriteLine("Suppose we have selected the first entry of the Expense type list in the UI: " + factory.Types[0]);
            Expense ex = factory.NewExpense(factory.Types[0]);
            Console.WriteLine("The new Expense has the type: " + ex.GetType().FullName);
        }

        static void CreateObjects()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BuildDatabase();

            Employee e = new Employee();
            e.FirstName = "John";
            e.LastName = "Becker";
            pm.MakePersistent(e);

            Travel t = e.NewTravel();
            t.Purpose = "NDO Workshop";

            ExpenseVoucher ev = new ExpenseVoucher();
            ev.VoucherText = "Taxi";
            ev.Sum = 30;
            ev.Date = DateTime.Now.Date;
            t.AddExpense(ev);

            MileageAllowance ma = new MileageAllowance();
            ma.MilesDriven = 200;
            ma.Date = DateTime.Now.Date;
            t.AddExpense(ma);

            PerDiemAllowance pda = new PerDiemAllowance();
            pda.Hours = (decimal)12.5;
            pda.Date = DateTime.Now.Date;
            t.AddExpense(pda);

            pm.Save();
        }
        static void QueryObjects()
        {
            Console.WriteLine("-------- Querying the objects ------------");
            PersistenceManager pm = new PersistenceManager();
            Employee e = (from em in pm.Objects<Employee>() select em).FirstOrDefault();
            
            Travel t = (Travel)e.Travels.FirstOrDefault();
            Console.WriteLine("Costs of the travel with the purpose " + t.Purpose + ":");
            foreach (Expense ex in t.Expenses)
                Console.WriteLine(ex.Date.ToShortDateString() + " " + ex.Text + " " + ex.Amount.ToString());
        }

		static void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}
    }
}
