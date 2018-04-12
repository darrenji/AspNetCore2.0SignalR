using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatOnConsole
{
    /// <summary>
    /// 这里相当于：当有一个Hub打开的时候，这里多出来一个客户端，也和该Hub的endpoint连接
    /// </summary>
    class Program
    {
        private static HubConnection _connection;
        static void Main(string[] args)
        {
            StartConnectionAsync();
            _connection.On<string, string>("tellEveryone", (name, message) =>
            {
                Console.WriteLine($"{name} said: {message}");
            });

            Console.ReadLine();
            DisposeAsync();
        }

        public static async Task StartConnectionAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:60584/chat")
                .WithConsoleLogger()
                 .Build();
            await _connection.StartAsync();
        }

        public static async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
