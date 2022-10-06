using System.Text.Json;

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

        [Test]
        public void Deserialize()
        {
            try
            {
                string json =
                    @"
                    {
                        ""controls"": [
                            {
                                ""name"": ""ValidityStartDate"",
                                ""label"": ""Date de début de validité"",
                                ""type"": ""date"",
                                ""value"": """",
                                ""sourceKeyName"": null,
                                ""destinationKeyName"": ""AccountCertificateExpirationDate"",
                                ""htmlTemplateKeyName"": null,
                                ""scamAgriOption"": 2,
                                ""scamProOption"": 2,
                                ""validators"": {
                                    ""minLength"": 10,
                                    ""maxLength"": 10
                                }
                            },
                            {
                                ""name"": ""ValidityExipirationDate"",
                                ""label"": ""Date de fin de validité"",
                                ""type"": ""date"",
                                ""value"": """",
                                ""sourceKeyName"": null,
                                ""destinationKeyName"": ""AccountCertificateExpirationDate"",
                                ""htmlTemplateKeyName"": null,
                                ""scamAgriOption"": 2,
                                ""scamProOption"": 2,
                                ""validators"": {
                                    ""minLength"": 10,
                                    ""maxLength"": 10
                                }
                            }
                        ]
                    }
                ";

                DeserializedObject parameters =
                    JsonSerializer.Deserialize<DeserializedObject>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    )!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class DeserializedObject
    {
        public List<DeserializedDetailObject>? Controls { get; set; }
    }

    public class DeserializedDetailObject
    {
        public string? Label { get; set; }
        public int? ScamAgriOption { get; set; }
        public int? ScamProOption { get; set; }

        public string ScamAgriOptionText
            => ScamAgriOption switch
            {
                0 => "Zéro",
                1 => "Un",
                2 => "Deux",

                _ => throw new NotImplementedException()
            };
    }
}