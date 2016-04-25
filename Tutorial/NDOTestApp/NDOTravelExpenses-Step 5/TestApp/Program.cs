//
// Copyright (c) 2002-2016 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


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
