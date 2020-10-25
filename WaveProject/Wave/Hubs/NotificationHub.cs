using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Log.Information("OnConnectedAsync");
            Log.Information($"Id: {this.Context.ConnectionId}");

            await base.OnConnectedAsync();
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, this.Context.UserIdentifier);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information("OnDisconnectedAsync");
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, this.Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
