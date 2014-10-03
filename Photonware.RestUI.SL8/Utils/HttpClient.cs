using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Photonware.RestUI.WP.Utils
{
    public class HttpClient
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
        public int Timeout { get; set; }

        public Photonware.RestUI.Utils.DefaultSshTunnelManager SshTunnelManager { get; set; }

        public HttpClient()
        {
            this.ResponseHeader = string.Empty;
            this.ResponseContent = string.Empty;
            this.RequestHeader = string.Empty;
            this.RequestContent = string.Empty;
            this.Timeout = 30000;
            this.SshTunnelManager = null;
        }

        public bool SendRequest(string request)
        {
            List<string> headers = new List<string>();
            HttpWebRequest hwr = null;
            string content = string.Empty;
            string proxyHost = string.Empty;
            int proxyPort = 0;

            using (StringReader reader = new StringReader(request))
            {
                string line = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        if (hwr == null)
                        {
                            continue;
                        }
                        else
                        {
                            content = reader.ReadToEnd();
                            break;
                        }
                    }
                    else if (line.StartsWith("#"))
                    {

                    }
                    else
                    {
                        if (hwr == null)
                        {
                            string regexStr = @"(.*?)\s+?(.*?)\s+?HTTP\/(\d+?)\.(\d+?)\s*?";

                            Match m = new Regex(regexStr).Match(line);
                            /*
                            string[] token = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (token.Length != 3)
                            {
                                throw new ArgumentException("the command line is wrong");
                            }
                             */


                            string host = null;

                            if (this.SshTunnelManager != null && this.SshTunnelManager.IsEnabled)
                            {
                                UriBuilder ub = new UriBuilder(m.Groups[2].Value);

                                SshTunnelManager.GetProxyAddress(ub.Uri.Host, ub.Uri.Port, out proxyHost, out proxyPort);

                                host = ub.Host;
                                ub.Host = proxyHost;
                                ub.Port = proxyPort;
                                //Console.WriteLine(proxyHost +"," +proxyPort);
                                hwr = HttpWebRequest.Create(ub.Uri) as HttpWebRequest;
                                //Console.WriteLine(hwr.RequestUri.ToString());

                            }
                            else
                            {
                                hwr = HttpWebRequest.Create(m.Groups[2].Value) as HttpWebRequest;
                                host = hwr.RequestUri.Host;
                            }
                            
                            hwr.Method = m.Groups[1].Value;
                            //hwr.ContentType = "application/xml";
                            hwr.UserAgent = "Simulator";
                            //this.SetHeader(hwr, "User-Agent", "Simulator");
                            //hwr.Headers["User-Agent"] = "Simulator";
                            //hwr.Host = host;
                            hwr.Headers["Host"] = host;
                            //this.SetHeader(hwr, "Host", host);
                            //hwr.Timeout = this.Timeout;
                            //hwr.ReadWriteTimeout = this.Timeout;
                            //hwr.ProtocolVersion = new Version(Convert.ToInt32(m.Groups[3].Value), Convert.ToInt32(m.Groups[4].Value));
                           
                            this.RequestHeader += line + "\r\n";
                        }
                        else
                        {
                            string[] token = line.Split(new char[] { ':' }, 2);
                            string header = token[0].ToLower().Trim();
                            switch (header)
                            {
                                case ("content-type"):
                                    hwr.ContentType = token[1].Trim();
                                    break;
                                case ("user-agent"):
                                    //hwr.Headers["User-Agent"] = token[1].Trim();
                                    hwr.UserAgent = token[1].Trim();
                                    //this.SetHeader(hwr, "User-Agent", token[1].Trim());
                                    break;
                                case ("host"):
                                    //hwr.Host = token[1].Trim();
                                    hwr.Headers["Host"] = token[1].Trim();
                                    //this.SetHeader(hwr, "Host", token[1].Trim());
                                    break;
                                case ("connection"):
                                    if (token[1].Trim().ToLower() == "keep-alive")
                                    {
                                        //hwr.KeepAlive = true;
                                        hwr.Headers["Connection"] = token[1].Trim();
                                        //this.SetHeader(hwr, "Connection", "Keep-Alive");
                                    }
                                    else
                                    {
                                        //hwr.Connection = token[1].Trim();
                                    }
                                    break;
                                case ("content-length"):
                                    break;
                                case ("keep-alive"):
                                    //hwr.Timeout = Convert.ToInt32(token[1].Trim()) * 1000;
                                    this.Timeout = Convert.ToInt32(token[1].Trim()) * 1000;
                                    hwr.Headers[token[0].Trim()] = token[1].Trim();
                                    break;
                                case ("expect"):
                                    //hwr.Expect = token[1].Trim();
                                    hwr.Headers["Expect"] = token[1].Trim();
                                    //this.SetHeader(hwr, "Expect", token[1].Trim());
                                    break;
                                case ("date"):
                                    //hwr.Date = DateTime.Parse(token[1].Trim());
                                    hwr.Headers["Date"] = token[1].Trim();
                                    //this.SetHeader(hwr, "Date", token[1].Trim());
                                    break;
                                case ("authorization"):
                                    string[] values = token[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (values[0].ToLower() == "real")
                                    {
                                        string[] userpass = values[1].Split(':');
                                        hwr.Credentials = new NetworkCredential(userpass[0], userpass[1]);
                                    }
                                    else
                                    {
                                        hwr.Headers[token[0].Trim()] = token[1].Trim();
                                    }
                                    break;
                                case ("accept"):
                                    hwr.Accept = token[1].Trim();
                                    break;
                                default:
                                    hwr.Headers[token[0].Trim()] = token[1].Trim();
                                    break;

                            }
                            this.RequestHeader += line + "\r\n";
                        }

                    }
                }

            }

            HttpWebResponse response = null;
            bool success = false;

            if (hwr != null)
            {
                try
                {


                    if (!string.IsNullOrEmpty(content))
                    {
                        byte[] buf = Encoding.UTF8.GetBytes(content);
                        hwr.ContentLength = buf.Length;

                        {
                            RequestData rd = new RequestData();
                            rd.Request = hwr;

                            IAsyncResult ar = hwr.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), rd);

                            rd.WaitHandler.WaitOne(this.Timeout);

                            if (rd.Success)
                            {
                                using (rd.RequestStream)
                                {
                                    rd.RequestStream.Write(buf, 0, buf.Length);
                                    rd.RequestStream.Flush();
                                }
                            }
                            else
                            {
                                throw rd.ExceptionMessage;
                            }
                        }
                    }

                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(string.Format("{0} {1} HTTP/{2}", hwr.Method, hwr.RequestUri.ToString(), "1.1"));
                        foreach (string key in hwr.Headers.AllKeys)
                        {
                            sb.AppendLine(string.Format("{0}: {1}", key, hwr.Headers[key]));
                        }

                        this.RequestHeader = sb.ToString() + "\r\n" + content + "\r\n";

                    }

                    {
                        RequestData rd = new RequestData();
                        rd.Request = hwr;

                        hwr.BeginGetResponse(new AsyncCallback(GetResponseCallback), rd);

                        rd.WaitHandler.WaitOne(this.Timeout);
                        //rd.WaitHandler.Close();

                        response = rd.Response;
                        //response = await hwr.GetResponseAsync() as HttpWebResponse;
                        success = rd.Success;
                    }

                }
                catch (WebException e)
                {
                    this.ResponseHeader = e.Message;
                    response = e.Response as HttpWebResponse;
                }

                try
                {

                    if (response != null)
                    {

                        using (response)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(string.Format("HTTP/{0} {1} {2}", "1.1", (int)response.StatusCode, response.StatusDescription));
                            this.StatusCode = (int)response.StatusCode;
                            foreach (string key in response.Headers.AllKeys)
                            {
                                sb.AppendLine(string.Format("{0}: {1}", key, response.Headers[key]));
                            }
                            this.ResponseHeader = sb.ToString();

                            using (Stream stream = response.GetResponseStream())
                            {

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    this.ResponseData = ms.ToArray();
                                }

                            }

                            if (response.ContentType.Contains("text/html") ||
                                response.ContentType.Contains("application/xml") ||
                                response.ContentType.Contains("application/json") ||
                                response.ContentType.Contains("text/plain") ||
                                response.ContentType.Contains("application/x-javascript") ||
                                response.ContentType.Contains("application/xhtml+xml") ||
                                response.ContentType.Contains("text/xml") ||
                                response.ContentType.Contains("application/javascript") ||
                                (string.IsNullOrEmpty(response.ContentType) && (
                                hwr.Accept.Contains("text/html") ||
                                hwr.Accept.Contains("application/xml") ||
                                hwr.Accept.Contains("application/json") ||
                                hwr.Accept.Contains("text/plain") ||
                                hwr.Accept.Contains("application/x-javascript") ||
                                hwr.Accept.Contains("application/xhtml+xml") ||
                                hwr.Accept.Contains("text/xml") ||
                                hwr.Accept.Contains("application/javascript"))))
                            {

                                Encoding encoding = Encoding.UTF8;
                                try
                                {
                                    if ((!string.IsNullOrEmpty(response.ContentType)) &&
                                        response.ContentType.IndexOf("charset", StringComparison.CurrentCultureIgnoreCase) >= 0)
                                    {

                                        //encoding = Encoding.GetEncoding(response.Headers[]);
                                    }
                                }
                                catch { }

                                using (MemoryStream stream = new MemoryStream(this.ResponseData))
                                {
                                    using (StreamReader reader = new StreamReader(stream, encoding))
                                    {
                                        this.ResponseContent = reader.ReadToEnd();
                                    }
                                }

                            }
                            else
                            {
                                this.ResponseContent = "The response cannot be decoded as string\r\nyou could access ResponseData for the response\r\n";
                            }
                        }


                        return success;
                    }
                }
                catch
                {
                    if (this.StatusCode == 200)
                    {
                        return success;
                    }
                }
                finally
                {
                    if (this.SshTunnelManager != null && SshTunnelManager.IsEnabled)
                    {
                        SshTunnelManager.ReleaseSshProxy(proxyHost, proxyPort);
                    }
                }

            }
            return false;
        }
        private void SetHeader(HttpWebRequest Request, string Header, string Value)
        {
            // Retrieve the property through reflection.
            PropertyInfo PropertyInfo = Request.GetType().GetRuntimeProperty(Header.Replace("-", string.Empty));
            // Check if the property is available.
            if (PropertyInfo != null)
            {
                // Set the value of the header.
                PropertyInfo.SetValue(Request, Value, null);
            }
            else
            {
                // Set the value of the header.
                Request.Headers[Header] = Value;
            }
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            RequestData rd = (RequestData)asynchronousResult.AsyncState;

            // End the operation
            try
            {
                rd.RequestStream = rd.Request.EndGetRequestStream(asynchronousResult);
                rd.Success = true;
            }
            catch (WebException we)
            {
                rd.Success = false;
                rd.ExceptionMessage = we;
            }
            finally
            {
                rd.WaitHandler.Set();
            }
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            RequestData rd = (RequestData)asynchronousResult.AsyncState;

            // End the operation

            try
            {
                rd.Response = (HttpWebResponse)rd.Request.EndGetResponse(asynchronousResult);
                rd.Success = true;

            }
            catch (WebException we) {
                rd.Response = (HttpWebResponse)we.Response;
                rd.Success = false;
                rd.ExceptionMessage = we;
            }
            finally
            {
                rd.WaitHandler.Set();
            }
        }
    }

    public class RequestData
    {
        public HttpWebRequest Request { get; set; }
        public HttpWebResponse Response { get; set; }
        public Stream RequestStream { get; set; }
        public Stream ResponseStream { get; set; }
        public ManualResetEvent WaitHandler { get; private set; }
        public WebException ExceptionMessage { get; set; }
        public bool Success { get; set; }
        public RequestData()
        {
            this.WaitHandler = new ManualResetEvent(false);
        }

        ~RequestData()
        {
            try
            {
                this.WaitHandler.Dispose();
            }
            catch { }
        }
    }
}
