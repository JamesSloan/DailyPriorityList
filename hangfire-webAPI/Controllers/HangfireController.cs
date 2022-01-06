using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace hangfire_webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HangfireController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello from hangfire web api");
        }

        //Fire and Forget job example
        [HttpPost]
        public IActionResult Welcome()
        {
            var jobId = BackgroundJob.Enqueue(() => SendMessage("Welcome to our app"));

            return Ok($"Job ID: {jobId}, Welcome email sent to the user");
        }

        //Delayed job example
        [HttpPost]
        public IActionResult Discount()
        {
            var delay = 120;
            var jobId = BackgroundJob.Schedule(() => SendMessage("Welcome to our app"), TimeSpan.FromSeconds(delay));

            return Ok($"Job ID: {jobId}, Discount email will be sent in {delay} seconds to the user");
        }

        //Repeated job example
        [HttpPost]
        public IActionResult DatabaseUpdate()
        {
            var frequency = 1;
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Database updated"), Cron.MinuteInterval(frequency));

            return Ok("Database update job initiated");
        }

        //Continuous job example - triggered when another job complete
        [HttpPost]
        public IActionResult ConfirmUnsubscribe()
        {
            var delay = 30;
            var jobId = BackgroundJob.Schedule(() => SendMessage("You asked to be unsubscribed"), TimeSpan.FromSeconds(delay));

            BackgroundJob.ContinueJobWith(jobId, () => SendMessage("Confirming that you have been unsubscribed"));

            return Ok("Confirmation job created");
        }

        public void SendMessage(string text)
        {
            Console.WriteLine(text);
        }
    }
}