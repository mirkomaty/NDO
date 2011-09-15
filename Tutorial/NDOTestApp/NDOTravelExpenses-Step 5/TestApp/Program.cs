using System;
using System.Collections;
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
			
            Image image = Image.FromFile(@"..\..\..\Building.bmp");
            Picture p = new Picture();
            p.Name = "Our first testpicture";
            p.CreationDate = DateTime.Now.Date;
            p.Image = image;
            PersistenceManager pm = new PersistenceManager();
            pm.BuildDatabase();
            pm.MakePersistent(p);
            pm.Save();
            /*
            PersistenceManager pm = new PersistenceManager();
            Query q = pm.NewQuery(typeof(PictureHeader), null);
            IList l = q.Execute();
            // Normally we'd show the headers in an UI
            foreach (PictureHeader ph in l)
                Console.WriteLine(ph.Name);

            // Select a PictureHeader and find the corresponding Picture
            Picture p = (Picture) pm.FindObject(typeof(Picture), ((PictureHeader)l[0]).NDOObjectId.Id.Value);
            Image image = p.Image;
            Console.WriteLine("Picture: " + p.Name + ": " + image.Height.ToString() + "x" + image.Width.ToString());
            */
        }

		static void CheckBits()
		{
			Console.WriteLine( "Running as " + (IntPtr.Size * 8) + " bit app." );
		}
    }
}
