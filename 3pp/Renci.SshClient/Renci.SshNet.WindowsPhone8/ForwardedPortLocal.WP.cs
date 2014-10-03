using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Renci.SshNet.Channels;
using Windows.Networking.Sockets;
using System.Threading.Tasks;

namespace Renci.SshNet
{
    /// <summary>
    /// Provides functionality for local port forwarding
    /// </summary>
    public partial class ForwardedPortLocal
    {
        private StreamSocketListener _listener;
        private readonly object _listenerLocker = new object();

        partial void ExecuteThread(Action action)
        {
            ThreadPool.QueueUserWorkItem((o) => { action(); });
        }

        partial void InternalStart()
        {
            //  If port already started don't start it again
            if (this.IsStarted)
                return;

            IPAddress addr = IPAddress.Parse(this.BoundHost);
            var ep = new IPEndPoint(addr, (int)this.BoundPort);

            this._listener = new StreamSocketListener();
            this._listener.ConnectionReceived += _listener_ConnectionReceived;

            this.Session.ErrorOccured += Session_ErrorOccured;
            this.Session.Disconnected += Session_Disconnected;

            this._listenerTaskCompleted = new ManualResetEvent(false);
            //this.ExecuteThread(() =>
            //{
                try
                {

                    //Task.Run(async () =>
                    //{
                    //    await this._listener.BindEndpointAsync(new Windows.Networking.HostName(this.BoundHost), "");
                    //}).Wait();
                    var a = this._listener.BindEndpointAsync(new Windows.Networking.HostName(this.BoundHost), "");
                    a.AsTask().Wait();

                    this.BoundPort = Convert.ToUInt32(this._listener.Information.LocalPort);
                }
                catch (SocketException exp)
                {
                    this.RaiseExceptionEvent(exp);
                }
                catch (Exception exp)
                {
                    this.RaiseExceptionEvent(exp);
                }
                finally
                {
                    this._listenerTaskCompleted.Set();
                }
            //});

            this.IsStarted = true;
        }

        void _listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                this.RaiseRequestReceived(args.Socket.Information.RemoteAddress.ToString(), Convert.ToUInt32(args.Socket.Information.RemotePort));
                using (var channel = this.Session.CreateClientChannel<ChannelDirectTcpipStreamSocket>())
                {
                    //channel.Open(this.Host, this.Port, socket);
                    channel.Open(this.Host, this.Port, args.Socket);

                    channel.Bind();

                    channel.Close();
                }
            }
            catch { }
        }

        partial void InternalStop()
        {
            //  If port not started you cant stop it
            if (!this.IsStarted)
                return;

            this.Session.Disconnected -= Session_Disconnected;
            this.Session.ErrorOccured -= Session_ErrorOccured;

            this.StopListener();

            this._listenerTaskCompleted.WaitOne(this.Session.ConnectionInfo.Timeout);
            this._listenerTaskCompleted.Dispose();
            this._listenerTaskCompleted = null;

            this.IsStarted = false;
        }
        private void StopListener()
        {
            lock (this._listenerLocker)
            {
                if (this._listener != null)
                {
                    this._listener.Dispose();
                    this._listener = null;
                }
            }
        }

        private void Session_ErrorOccured(object sender, Common.ExceptionEventArgs e)
        {
            this.StopListener();
        }

        private void Session_Disconnected(object sender, EventArgs e)
        {
            this.StopListener();
        }
    }
}
