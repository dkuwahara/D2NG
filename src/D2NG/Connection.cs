using Serilog;
using Stateless;
using System.Net;
using System.Net.Sockets;

namespace D2NG
{
    internal abstract class Connection
    {
        protected enum State
        {
            NotConnected,
            Connected
        }

        protected enum Trigger
        {
            ConnectSocket,
            Terminate,
            Write,
            Read
        }

        /**
         * State Machine for the connection
         */
        protected readonly StateMachine<State, Trigger> _machine;

        private readonly StateMachine<State, Trigger>.TriggerWithParameters<IPAddress, int> _connectTrigger;


        /**
        * The actual TCP Connection
        */
        protected NetworkStream _stream;

        protected TcpClient _tcpClient;


        protected Connection()
        {
            _machine = new StateMachine<State, Trigger>(State.NotConnected);
            _connectTrigger = _machine.SetTriggerParameters<IPAddress, int>(Trigger.ConnectSocket);

            _machine.Configure(State.NotConnected)
                .OnEntryFrom(Trigger.Terminate, OnTerminate)
                .Permit(Trigger.ConnectSocket, State.Connected);

            _machine.Configure(State.Connected)
                .OnEntryFrom(_connectTrigger, (ip, port) => OnConnect(ip, port))
                .Permit(Trigger.Terminate, State.NotConnected);
        }

        internal abstract byte[] ReadPacket();

        internal abstract void WritePacket(byte[] packet);

        public void Connect(IPAddress ip, int port) => _machine.Fire(_connectTrigger, ip, port);

        protected void OnConnect(IPAddress ip, int port)
        {
            Log.Debug("[{0}] Connecting to {1}:{2}", GetType(), ip, port);
            _tcpClient = new TcpClient();
            _tcpClient.Connect(ip, port);
            _stream = _tcpClient.GetStream();
            if (!_stream.CanWrite)
            {
                Log.Debug("[{0}] Unable to write to {1}:{2}, closing connection", GetType(), ip, port);
                Terminate();
                throw new UnableToConnectException($"Unable to establish {GetType()}");
            }
            _stream.WriteByte(0x01);
            Log.Debug("[{0}] Successfully connected to {1}:{2}", GetType(), ip, port);
        }

        public bool Connected => _machine.IsInState(State.Connected);

        public void Terminate() => _machine.Fire(Trigger.Terminate);

        protected void OnTerminate()
        {
            _tcpClient.Close();
            _stream.Close();
        }

        public void WritePacket(Packet packet) => this.WritePacket(packet.Raw);

    }
}
