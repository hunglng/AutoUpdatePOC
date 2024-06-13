using Microsoft.AspNetCore.SignalR;

namespace AutoUpdatePOC.UpdateHub
{
    public class UpdateHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task DownloadCompleted()
        {
            await Clients.Caller.SendAsync("UpdateAndReload");
        }
    }
}
