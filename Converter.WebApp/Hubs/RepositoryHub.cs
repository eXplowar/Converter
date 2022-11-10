using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Converter.WebApp.Hubs
{
    /// <summary>
    /// This hub stores and informs clients about completed conversions.
    /// </summary>
    public class RepositoryHub : Hub
    {
        private ConcurrentDictionary<string, int> MessagesForSend { get; set; } = new();

        /// <summary>
        /// Sending a message about the completed conversion.
        /// </summary>
        /// <param name="hubCallerContext">HubCallerContext</param>
        /// <param name="fileHash">File identifier</param>
        /// <param name="token">User identifier</param>
        /// <returns></returns>
        public async Task SendMessage(HubCallerContext hubCallerContext, int fileHash, string token)
        {
            if (Clients is not null && Clients.Client(hubCallerContext.ConnectionId) is IClientProxy client)
            {
                await client.SendAsync("CompletedConversion", fileHash, token);
                RemoveTokenFromMessages(token);
                return;
            }

            // If at the time of sending the message there is no connected user - add the result to the list of messages,
            // from which in the future the newly connected user will be able to receive his message
            MessagesForSend.TryAdd(token, fileHash);
        }

        /// <summary>
        /// Web client registration after connecting to the hub.
        /// <para>If the repository has ready files for the user - send a message.</para>
        /// </summary>
        /// <param name="token">User identifier</param>
        /// <returns></returns>
        public async Task RegisterClient(string token)
        {
            var client = Clients.Client(Context.ConnectionId);

            if (MessagesForSend.TryGetValue(token, out var fileHash))
            {
                await client.SendAsync("CompletedConversion", fileHash, token);
                RemoveTokenFromMessages(token);
            }
        }

        /// <summary>
        /// Remove the token after receiving the message.
        /// </summary>
        /// <param name="token">User identifier</param>
        private void RemoveTokenFromMessages(string token)
        {
            if (MessagesForSend.ContainsKey(token))
            {
                MessagesForSend.TryRemove(token, out _);
            }
        }
    }
}
