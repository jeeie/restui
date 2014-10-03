using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Photonware.RestUI.Actions
{
    public class RestAction : Photonware.RestUI.Core.Action
    {

        public RestAction() : base()
        {
            Key = "rest_action";
            Text = "RESTful Action";
            Description = "send REST request";
            
            this.Xshd = Photonware.RestUI.Actions.Properties.Resources.Rest;
            //_addExamples();
        }


        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            throw new NotImplementedException();
        }
    }

    public class RestActionResult
    {
        public int StatusCode { get; private set; }
        public string RequestHeader { get; private set; }
        public string RequestContent { get;private set; }
        public string ResponseHeader { get; private set; }
        public string ResponseContent { get; private set; }
        public List<byte> ResponseData { get; private set; }

        public RestActionResult(int statusCode, string requestHeader, string requestContent, string responseHeader, string responseContent, byte[] responseData)
        {
            this.StatusCode = statusCode;
            this.RequestHeader = requestHeader;
            this.RequestContent = requestContent;
            this.ResponseHeader = responseHeader;
            this.ResponseContent = responseContent;
            if (responseData != null)
            {
                this.ResponseData = responseData.ToList<byte>();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", RequestHeader, RequestContent, ResponseHeader, ResponseContent);
        }

    }
}
