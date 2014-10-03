using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace Photonware.RestUI.SL8.Xamls
{
    public class PhotoCameraLuminanceSource : BaseLuminanceSource
    {
        public byte[] PreviewBufferY
        {
            get
            {
                return luminances;
            }
        }

        public PhotoCameraLuminanceSource(int width, int height)
            : base(width, height)
        {
            luminances = new byte[width * height];
        }

        internal PhotoCameraLuminanceSource(int width, int height, byte[] newLuminances)
            : base(width, height)
        {
            luminances = newLuminances;
        }

        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
        {
            return new PhotoCameraLuminanceSource(width, height, newLuminances);
        }
    }
}
