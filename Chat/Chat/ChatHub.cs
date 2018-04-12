using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Chat
{
    public class ChatHub:Hub
    {
        //由客户端调用这里的方法，提供这里的参数
        public void Send(string name, string message)
        {
            //所有的客户端都调用客户端方法tellEveryone
            Clients.All.InvokeAsync("tellEveryone", name, message);
        }

        public override Task OnConnectedAsync()
        {
            //原来从Hub从可以拿到Context属性，而这个属性中包含了一些很重要的数据，比如ConnectionId
            Clients.All.InvokeAsync("tellEveryone", "system", $"{Context.ConnectionId} joined this conversation");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.InvokeAsync("tellEveryone", "system", $"{Context.ConnectionId} left this conversation");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
