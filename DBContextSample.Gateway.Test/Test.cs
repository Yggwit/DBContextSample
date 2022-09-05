using Microsoft.Extensions.Configuration;

namespace DBContextSample.Gateway.Test
{
    public class Test
    {
        private readonly HttpClient _client = new();

        [SetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            string clientUrl = configuration.GetValue<string>("Client:Url");

            _client.BaseAddress = new Uri(clientUrl);
        }


        [Test]
        public async Task Http_Test1()
        {
            try
            {
                var response = await _client.GetAsync("/api/people");

                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                var people = await response.Content.ReadAsStringAsync();

                Assert.That(people, Is.Not.Null);
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.Null);
            }
        }

        [Test]
        public async Task Http_Test2()
        {
            try
            {
                await Task.WhenAll(
                    _client.GetAsync("/api/people"),
                    _client.GetAsync("/api/people"),
                    _client.GetAsync("/api/people"),
                    _client.GetAsync("/api/people"),
                    _client.GetAsync("/api/people"),
                    _client.GetAsync("/api/people")
                );
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.Null);
            }
        }
    }
}