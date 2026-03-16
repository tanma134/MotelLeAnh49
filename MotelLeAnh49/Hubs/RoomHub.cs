using Microsoft.AspNetCore.SignalR;

namespace MotelLeAnh49.Hubs
{
    public class RoomHub : Hub
    {
        public async Task SendRoomUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveRoomUpdate", message);
        }
    }
}