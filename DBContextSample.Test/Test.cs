namespace DBContextSample.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        [TestCase(20)]
        public void Fizzbuzz(int i)
        {
            List<(int, string)> res = new();

            for (int j = 1; j < i; j++)
                if (
                    $"{(j % 3 == 0 ? "fizz" : "")}{(j % 5 == 0 ? "buzz" : "")}" is string fb
                    && !string.IsNullOrEmpty(fb)
                )
                    res.Add((j, fb));
        }
    }
}