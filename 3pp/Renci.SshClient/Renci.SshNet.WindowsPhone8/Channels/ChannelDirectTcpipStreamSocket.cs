using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Renci.SshNet.Common;
using Renci.SshNet.Messages.Connection;
using Windows.Networking.Sockets;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Renci.SshNet.Channels
{
    /// <summary>
    /// Implements "direct-tcpip" SSH channel.
    /// </summary>
    internal partial class ChannelDirectTcpipStreamSocket : ClientChannel
    {
        private EventWaitHandle _channelEof = new AutoResetEvent(false);
        private EventWaitHandle _channelOpen = new AutoResetEvent(false);
        private EventWaitHandle _channelData = new AutoResetEvent(false);
        private StreamSocket _streamSocket;

        /// <summary>
        /// Gets the type of the channel.
        /// </summary>
        /// <value>
        /// The type of the channel.
        /// </value>
        public override ChannelTypes ChannelType
        {
            get { return ChannelTypes.DirectTcpip; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDirectTcpip"/> class.
        /// </summary>
        public ChannelDirectTcpipStreamSocket()
        {
        }

        public void Open(string remoteHost, uint port, StreamSocket socket)
        {
            this._streamSocket = socket;

            //var ep = socket.RemoteEndPoint as IPEndPoint;

            if (!this.IsConnected)
            {
                throw new SshException("Session is not connected.");
            }

            //  Open channel
            this.SendMessage(new ChannelOpenMessage(
                this.LocalChannelNumber, this.LocalWindowSize, this.LocalPacketSize,
                new DirectTcpipChannelInfo(remoteHost, port, 
                    this._streamSocket.Information.RemoteAddress.ToString(), 
                    Convert.ToUInt32(this._streamSocket.Information.RemotePort))));

            //  Wait for channel to open
            this.WaitOnHandle(this._channelOpen);
        }

        /// <summary>
        /// Binds channel to remote host.
        /// </summary>
        public void Bind()
        {
            //  Cannot bind if channel is not open
            if (!this.IsOpen)
                return;

            //  Start reading data from the port and send to channel
            Exception exception = null;

            try
            {
                //var buffer = new byte[this.RemotePacketSize];
                byte[] buffer = null;

                while (this._streamSocket != null)
                {
                    try
                    {
                        var read = 0;
                        this.InternalSocketReceive(out buffer, ref read);
                        if (read > 0)
                        {
                            this.SendMessage(new ChannelDataMessage(this.RemoteChannelNumber, buffer.Take(read).ToArray()));
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (SocketException exp)
                    {
                        /*
                        if (exp.SocketErrorCode == SocketError.WouldBlock ||
                            exp.SocketErrorCode == SocketError.IOPending ||
                            exp.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                        {
                            // socket buffer is probably empty, wait and try again
                            Thread.Sleep(30);
                        }
                        else if (exp.SocketErrorCode == SocketError.ConnectionAborted || exp.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            break;
                        }
                        else
                            throw;  // throw any other error
                         */
                    }
                }
            }
            catch (Exception exp)
            {
                exception = exp;
            }

            //  Channel was open and we MUST receive EOF notification,
            //  data transfer can take longer than connection specified timeout
            //  If listener thread is finished then socket was closed
            WaitHandle.WaitAny(new WaitHandle[] {_channelEof});

            //  Close socket if still open
            if (this._streamSocket != null)
            {
                this._streamSocket.Dispose();
                this._streamSocket = null;
            }

            if (exception != null)
                throw exception;
        }

        public override void Close()
        {
            //  Close socket if still open
            if (this._streamSocket != null)
            {
                this._streamSocket.Dispose();
                this._streamSocket = null;
            }

            //  Send EOF message first when channel need to be closed
            this.SendMessage(new ChannelEofMessage(this.RemoteChannelNumber));

            base.Close();
        }

        /// <summary>
        /// Called when channel data is received.
        /// </summary>
        /// <param name="data">The data.</param>
        protected override void OnData(byte[] data)
        {
            base.OnData(data);

            this.InternalSocketSend(data);
        }

        /// <summary>
        /// Called when channel is opened by the server.
        /// </summary>
        /// <param name="remoteChannelNumber">The remote channel number.</param>
        /// <param name="initialWindowSize">Initial size of the window.</param>
        /// <param name="maximumPacketSize">Maximum size of the packet.</param>
        protected override void OnOpenConfirmation(uint remoteChannelNumber, uint initialWindowSize, uint maximumPacketSize)
        {
            base.OnOpenConfirmation(remoteChannelNumber, initialWindowSize, maximumPacketSize);

            this._channelOpen.Set();
        }

        protected override void OnOpenFailure(uint reasonCode, string description, string language)
        {
            base.OnOpenFailure(reasonCode, description, language);

            this._channelOpen.Set();
        }

        /// <summary>
        /// Called when channel has no more data to receive.
        /// </summary>
        protected override void OnEof() {
	        base.OnEof();

            var channelEof = this._channelEof;
            if (channelEof != null)
                channelEof.Set();
        }

        protected override void OnClose()
        {
            base.OnClose();

            var channelEof = this._channelEof;
            if (channelEof != null)
                channelEof.Set();
        }

        protected override void OnErrorOccured(Exception exp)
        {
            base.OnErrorOccured(exp);

            //  If error occured, no more data can be received
            var channelEof = this._channelEof;
            if (channelEof != null)
                channelEof.Set();
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            //  If disconnected, no more data can be received
            var channelEof = this._channelEof;
            if (channelEof != null)
                channelEof.Set();
        }

        private void ExecuteThread(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }

        private void InternalSocketReceive(out byte[] buffer, ref int read)
        {
            var buf = new Windows.Storage.Streams.Buffer((uint)this.RemotePacketSize);

            var aa = this._streamSocket.InputStream.ReadAsync(buf, buf.Capacity, InputStreamOptions.Partial);
            aa.AsTask().Wait();
            var buf1 = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buf);
            buffer = buf1;
            read = buf1.Length;
      
            

            /*
            using (Windows.Storage.Streams.DataReader dr = new Windows.Storage.Streams.DataReader(this._streamSocket.InputStream))
            {
                dr.ByteOrder = ByteOrder.LittleEndian;
              
                var a = dr.LoadAsync(sizeof(uint));
                a.AsTask().Wait();

                uint len = dr.ReadUInt32();

                byte[] buf = new byte[len];
                dr.ReadBytes(buf);

                buf.CopyTo(buffer, 0);

                dr.DetachStream();
            }
             */
        }

        private void InternalSocketSend(byte[] data)
        {
            using (Windows.Storage.Streams.DataWriter dw = new Windows.Storage.Streams.DataWriter(this._streamSocket.OutputStream))
            {
                Task.Run(async () =>
                {
                    dw.WriteBytes(data);
                    await dw.StoreAsync();
                    await dw.FlushAsync();
                }).Wait();

                dw.DetachStream();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this._streamSocket != null)
            {
                this._streamSocket.Dispose();
                this._streamSocket = null;
            }

            if (this._channelEof != null)
            {
                this._channelEof.Dispose();
                this._channelEof = null;
            }

            if (this._channelOpen != null)
            {
                this._channelOpen.Dispose();
                this._channelOpen = null;
            }

            if (this._channelData != null)
            {
                this._channelData.Dispose();
                this._channelData = null;
            }

            base.Dispose(disposing);
        }
    }
}
