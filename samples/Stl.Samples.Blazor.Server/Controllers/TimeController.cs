using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Bridge;
using Stl.Fusion.Server;
using Stl.Samples.Blazor.Common.Services;

namespace Stl.Samples.Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeController : FusionController, ITimeService
    {
        private readonly ITimeService _time;

        public TimeController(ITimeService time, IPublisher publisher)
            : base(publisher) 
            => _time = time;

        [HttpGet("get")]
        public async Task<DateTime> GetTimeAsync(CancellationToken cancellationToken)
        {
            var c = await HttpContext.TryPublishAsync(Publisher, 
                _ => _time.GetTimeAsync(cancellationToken),
                cancellationToken);
            return c.Value;
        }
    }
}
