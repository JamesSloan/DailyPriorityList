using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace hangfire_webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangfireController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from hangfire web api");
        }

        //Fire and Forget job example
        [HttpPost]
        [Route("[action]")]
        public IActionResult Welcome()
        {
            var jobId = BackgroundJob.Enqueue(() => SendMessage("Welcome to our app"));

            return Ok($"Job ID: {jobId}, Welcome email sent to the user");
        }

        //Delayed job example
        [HttpPost]
        [Route("[action]")]
        public IActionResult Discount()
        {
            var delay = 120;
            var jobId = BackgroundJob.Schedule(() => SendMessage("Welcome to our app"), TimeSpan.FromSeconds(delay));

            return Ok($"Job ID: {jobId}, Discount email will be sent in {delay} seconds to the user");
        }

        //Repeated job example
        [HttpPost]
        [Route("[action]")]
        public IActionResult DatabaseUpdate()
        {
            var frequency = 1;
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Database updated"), Cron.MinuteInterval(frequency));

            return Ok("Database update job initiated");
        }

        public void SendWelcomeEmail(string text)
        {
            Console.WriteLine(text);
        }
    }
}