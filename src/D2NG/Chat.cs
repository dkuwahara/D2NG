using D2NG.BNCS;
using System.Linq;

namespace D2NG
{
    /// <summary>
    /// Interface for interacting with chat packets for BNCS
    /// </summary>
    public class Chat
    {
        private BattleNetChatServer _bncs;

        internal Chat(BattleNetChatServer bncs)
        {
            this._bncs = bncs;
        }

        /// <summary>
        /// Join a channel
        /// </summary>
        /// <param name="channel">Name of the channel to join. Maximum 31 characters</param>
        public void JoinChannel(string channel)
        {
            _bncs.JoinChannel(channel);
        }

        /// <summary>
        /// Send a chat message or a command to Battle.net
        /// </summary>
        /// <param name="message">
        /// Message to send to Battle.net, should only accept chars where the value is greater than 0x20. 
        /// Maximum length is 223 characters. Any additional characters will be truncated
        /// </param>
        public void Send(string message)
        {
            if (message.ToCharArray().Any(c => c < 0x20))
            {
                throw new ChatValidationException("Message contains invalid characters");
            }
            _bncs.ChatCommand(message.Trim());
        }
    }
}