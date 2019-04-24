using Stateless;
using System;
using System.Collections.Generic;
using System.Text;

namespace D2NG.BNLS
{
    class BnlsClient
    {

        private readonly StateMachine<State, Trigger> _machine = new StateMachine<State, Trigger>(State.NotConnected);

        private enum State
        {
            NotConnected,
            Connected,
            Authenticated
        }

        private enum Trigger
        {
            Connect,
            Authenticate,
            Disconnect
        }

        public BnlsClient()
        {
            _machine.Configure(State.NotConnected)
                .Permit(Trigger.Connect, State.Connected)
                .OnEntryFrom(Trigger.Disconnect, () => OnDisconnect());

            _machine.Configure(State.Connected)
                .Permit(Trigger.Disconnect, State.NotConnected)
                .Permit(Trigger.Authenticate, State.Authenticated)
                .OnEntryFrom(Trigger.Connect, () => OnConnect());

            _machine.Configure(State.Authenticated)
                .SubstateOf(State.Connected)
                .Permit(Trigger.Disconnect, State.NotConnected);
        }

        public void Connect(String server) => _machine.Fire(Trigger.Connect);
        public void Disconnect() => _machine.Fire(Trigger.Disconnect);
        public void Authenticate(String user, String password) => _machine.Fire(Trigger.Authenticate);

        private void OnDisconnect()
        {
            throw new NotImplementedException();
        }

        private void OnConnect()
        {
            throw new NotImplementedException();
        }

        public byte RequestVersionByte(String product)
        {
            if(_machine.IsInState(State.Authenticated))
            {
                return 0x0e;
            } else
            {
                throw new Exception("Client is not authenticated");
            }
        }

        public VersionInfo CheckVersion(String Product, long ftime, String filename, String value)
        {
            if (_machine.IsInState(State.Authenticated))
            {
                return null;
            }
            else
            {
                throw new Exception("Client is not authenticated");
            }
        }

 
    }
}
