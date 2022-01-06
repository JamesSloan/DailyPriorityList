using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace hangfire_webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HangfireController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string applicationUrl = "https://localhost:7179/api";

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

        [HttpGet("{username}")]
        public async Task<IActionResult> Login(string username)
        {
            BackgroundJob.Enqueue(() => SendMessage($"Welcome to our app {username}"));

            var response = await getValues(username);
            var values = response;
            return Ok($"{username} logged in, with values {values}");
        }

        private async Task<string> getValues(string username)
        {
            var requestUrl = $"{applicationUrl}/Values/{username}";
            Console.WriteLine(requestUrl);
            try
            {
                var valuesResponse = await client.GetAsync(requestUrl);
                valuesResponse.EnsureSuccessStatusCode();
                var values = valuesResponse.Content.ReadAsStringAsync().Result;
                return values;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendMessage(string text)
        {
            Console.WriteLine(text);
        }
    }
}