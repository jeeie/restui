using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Photonware.RestUI.Utils
{
    public abstract class HttpClient
    {

        public int Timeout { get; set; }

        public HttpClient()
        {
            this.Timeout = 30000;
        }

        public abstract bool SendRequest(string request, out HttpResult result);
    }

    public class HttpResult
    {
        public string ResponseHeader
        {
            get;
            private set;
        }

        public string ResponseContent
        {
            get;
            private set;
        }

        public string RequestHeader
        {
            get;
            private set;

        }

        public string RequestContent
        {
            get;
            private set;

        }

        public byte[] ResponseData
        {
            get;
            private set;
        }

        public int StatusCode
        {
            get;
            private set;
        }

        public HttpResult()
        {
            this.ResponseHeader = string.Empty;
            this.ResponseContent = string.Empty;
            this.RequestHeader = string.Empty;
            this.RequestContent = string.Empty;
        }
    }
}
