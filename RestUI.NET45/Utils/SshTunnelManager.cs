using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestUI.Utils
{
    public class SshTunnelManager : Photonware.RestUI.Utils.DefaultSshTunnelManager
    {
        private static readonly SshTunnelManager manager = new SshTunnelManager();
        public static SshTunnelManager Instance { get { return manager; } }
        private SshTunnelManager()
        {
            this.init();
        }

        private void init()
        {
            this.host = string.Empty;
            this.port = 22;
            this.username = string.Empty;
            this.password = string.Empty;
        }

        private SshClient sshClient = null;
        private Dictionary<string, ForwardedPortLocal> forwardedPorts = new Dictionary<string, ForwardedPortLocal>();


        public object SyncRoot { get { return this; } }



        public override void SetParameters(string host, int port, string username, string password)
        {
            this.host = host; this.port = port; this.username = username; this.password = password;
            lock (this.SyncRoot)
            {
                this.changed = true;
            }

        }

        public override void CloseSshClient()
        {
            lock (this.SyncRoot)
            {
                if (sshClient != null)
                {
                    try
                    {
                        List<ForwardedPort> ports = new List<ForwardedPort>();
                        foreach (ForwardedPort p in sshClient.ForwardedPorts)
                        {
                            //sshClient.RemoveForwardedPort(p);
                            ports.Add(p);
                        }

                        foreach (ForwardedPort p in ports)
                        {
                            sshClient.RemoveForwardedPort(p);
                        }

                        forwardedPorts.Clear();

                        sshClient.ErrorOccurred -= sshClient_ErrorOccurred;

                        if (sshClient.IsConnected)
                        {
                            sshClient.Disconnect();
                        }
                        sshClient.Dispose();

                    }
                    catch
                    {

                    }
                    finally
                    {
                        try
                        {
                            sshClient.Dispose();
                        }
                        catch { }
                        sshClient = null;
                    }
                }
            }
        }

        void sshClient_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            
        }

        public override bool GetProxyAddress(string targetHost, int targetPort, out string proxyHost, out int proxyPort)
        {
            
            try
            {
                lock (this.SyncRoot)
                {
                    if (changed)
                    {

                        this.CloseSshClient();
                        sshClient = new SshClient(host, port, username, password);
                        this.forwardedPorts.Clear();
                        this.changed = false;

                    }
                    if (!sshClient.IsConnected)
                    {
                        List<ForwardedPort> ports = new List<ForwardedPort>();
                        foreach (ForwardedPort p in sshClient.ForwardedPorts)
                        {
                            //sshClient.RemoveForwardedPort(p);
                            ports.Add(p);
                        }

                        foreach (ForwardedPort p in ports)
                        {
                            sshClient.RemoveForwardedPort(p);
                        }
                        forwardedPorts.Clear();
                        sshClient.Connect();
                        sshClient.KeepAliveInterval = TimeSpan.FromSeconds(60);

                    }
                }

                ForwardedPortLocal forwardedPort = null;

                // check if the targetHost and targetPort have been created
                lock (this.SyncRoot)
                {
                    
                    //string key = targetHost + "||" + targetPort;
                    //if (forwardedPorts.ContainsKey(key))
                    //{
                    //    forwardedPort = forwardedPorts[key];
                   // }
                    //else
                    //{
                        forwardedPort = new ForwardedPortLocal("127.0.0.1", 0, targetHost, (uint)targetPort);

                        sshClient.AddForwardedPort(forwardedPort);

                    //    forwardedPorts.Add(key, forwardedPort);

                        forwardedPort.Start();
                        
                   // }

                    if (!forwardedPort.IsStarted)
                    {

                        forwardedPort.Start();

                    }

                    proxyHost = forwardedPort.BoundHost;
                    proxyPort = (int)forwardedPort.BoundPort;

                }

                return true;
            }
            finally { }
        }

        public override void ReleaseSshProxy(string proxyHost, int proxyPort)
        {
            lock (this.SyncRoot)
            {
                if (sshClient == null) return;
                if (!this.IsEnabled) return;
                ForwardedPort fwPort = null;
                foreach (ForwardedPort p in sshClient.ForwardedPorts)
                {
                    ForwardedPortLocal port = p as ForwardedPortLocal;
                    if (port.BoundHost == proxyHost && port.BoundPort == proxyPort)
                    {
                        fwPort = p;
                        break;
                    }
                }
                if (fwPort != null)
                {
                    try
                    {
                        //fwPort.Stop();
                        sshClient.RemoveForwardedPort(fwPort);
                    }
                    catch { }
                }
            }
        }

    }
}
