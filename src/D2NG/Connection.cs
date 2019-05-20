using Stateless;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace D2NG
{
    class Connection
    {
        protected enum State
        {
            NotConnected,
            Connected
        }

        protected enum Trigger
        {
            ConnectSocket,
            KillSocket,
            Write,
            Read
        }

        /**
         * State Machine for the connection
         */
        protected readonly StateMachine<State, Trigger> _machine;

        /**
        * The actual TCP Connection
        */
        protected NetworkStream _stream;

        protected TcpClient _tcpClient;


        public Connection()
        {
            _machine = new StateMachine<State, Trigger>(State.NotConnected);

            _machine.Configure(State.NotConnected)
                .Permit(Trigger.ConnectSocket, State.Connected);

            _machine.Configure(State.Connected)
                .Permit(Trigger.KillSocket, State.NotConnected);
        }
    }
}
