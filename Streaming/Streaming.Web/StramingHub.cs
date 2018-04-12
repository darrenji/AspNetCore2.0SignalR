using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Streaming.Web
{
    public class StramingHub : Hub
    {
        public void SendStreamInit()
        {
            Clients.All.InvokeAsync("streamStarted");
        }

        public IObservable<string> StartStreaming()
        {
            return Observable.Create(
                async (IObserver<string> observer) =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        observer.OnNext($"sending...{i}");
                        await Task.Delay(1000);
                    }
                }
                );
        }
    }
}
