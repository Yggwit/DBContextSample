namespace DBContextSample.Gateway.Test
{
    public class Test
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var application = new ApplicationFactory();

            _client = application.CreateClient();
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
    }
}