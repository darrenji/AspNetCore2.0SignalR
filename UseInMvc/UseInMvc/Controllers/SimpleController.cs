using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UseInMvc.Controllers
{
    [Route("api/[controller]")]
    public class SimpleController : Controller
    {
        private IHubContext<NotificationHub> _hubContext;

        public SimpleController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;  
        }

        [HttpGet]
        public string Get()
        {
            _hubContext.Clients.All.InvokeAsync("updateStuff", "some random text");
            return "I have been called!";
        }
    }
}
