using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Utils
{
    public abstract class DefaultSshTunnelManager
    {

        protected bool changed = false;
        protected string host = string.Empty;
        protected int port = 22;
        protected string username = string.Empty;
        protected string password = string.Empty;


        public bool IsEnabled
        {
            get;
            set;
        }

        public virtual void SetParameters(string host, int port, string username, string password)
        {
            this.host = host; this.port = port; this.username = username; this.password = password;
        }

        public abstract void CloseSshClient();


        public abstract bool GetProxyAddress(string targetHost, int targetPort, out string proxyHost, out int proxyPort);

        public abstract void ReleaseSshProxy(string proxyHost, int proxyPort);

    }
}
