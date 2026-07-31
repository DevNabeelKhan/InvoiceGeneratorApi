using Microsoft.AspNetCore.SignalR;

namespace ConvergeAPI.Hubs
{
    public class SignalRNotificationHub:Hub
    {
        public async Task JoinGroup(string Type)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Type);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
        //public async Task SendNotification(Notification notification)
        //{
        //    await Clients.All.SendAsync("ReceiveNotification", notification);
        //    Console.WriteLine("Notification sent");
        //}
        //public async Task SendNotification(Notification notification)
        //{
        //    await Clients.All.SendAsync("All", notification);
        //    await Clients.All.SendAsync("AllFaculty", notification);
        //    await Clients.All.SendAsync("AllStudent", notification);
        //    await Clients.All.SendAsync("ReceiveNotification", notification);
        //}
    }
}