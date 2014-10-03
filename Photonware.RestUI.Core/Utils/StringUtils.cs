using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core.Utils
{
    public class StringUtils
    {
        public string GetUTF8String(object obj)
        {
            System.Diagnostics.Debug.WriteLine(obj.ToString());
            byte[] bytes = obj as byte[];
            if (bytes != null)
            {
                return UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }

            return "null";
        }

        public string GetUTF8String(List<byte> buf)
        {
            var bytes = buf.ToArray();
            if (bytes != null)
            {
                return UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
            return "null";
        }
    }
}
