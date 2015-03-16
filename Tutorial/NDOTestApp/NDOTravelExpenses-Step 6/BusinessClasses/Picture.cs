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
