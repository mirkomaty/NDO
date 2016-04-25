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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NDO;

namespace BusinessClasses
{
    [NDOPersistent]
    public partial class Picture : PictureHeader
    {
        byte[] rawbytes = null;
        [NDOTransient]
        // can not be stored directly
        Image image = null;
        public Image Image
        {
            get
            {
                if (rawbytes == null)
                    return null;
                if (image == null)
                {
                    System.IO.MemoryStream ms = new MemoryStream(rawbytes);
                    image = Image.FromStream(ms);
                }
                return image;
            }
            set
            {
                image = value;
                if (image == null)
                {
                    rawbytes = null;
                    return;
                }
                // horizontal align to 4 bytes 
                int horsize = image.Size.Width * 3;
                int rest = horsize % 4;
                if (rest > 0) rest = 1;
                horsize = horsize / 4;
                horsize = (horsize + rest) * 4;
                int vertsize = image.Size.Height;
                rawbytes = new byte[horsize * vertsize + 56];
                System.IO.MemoryStream ms = new MemoryStream(rawbytes);
                image.Save(ms, ImageFormat.Bmp);
            }
        }
        public Picture()
        {
        }
    }
}
