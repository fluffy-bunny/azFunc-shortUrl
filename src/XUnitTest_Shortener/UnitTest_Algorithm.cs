using dotnetcore.keyvault.fetch;
using dotnetcore.urlshortener.contracts.Models;
using dotnetcore.urlshortener.generator;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace XUnitTest_Shortener
{
    public class UnitTest_Algorithm
    {
        [Fact]
        public void Base64Decode_test()
        {
            /*
             * {"authority":"https://apim-organics.azure-api.net/oauth2","tenants":[{"credentials":{"client_id":"b2b-client","client_secret":"secret"},"name":"marketing","options":{"max_ttl":2592000}}]}
             */
            var encoded = "eyJhdXRob3JpdHkiOiJodHRwczovL2FwaW0tb3JnYW5pY3MuYXp1cmUtYXBpLm5ldC9vYXV0aDIiLCJ0ZW5hbnRzIjpbeyJjcmVkZW50aWFscyI6eyJjbGllbnRfaWQiOiJiMmItY2xpZW50IiwiY2xpZW50X3NlY3JldCI6InNlY3JldCJ9LCJuYW1lIjoibWFya2V0aW5nIiwib3B0aW9ucyI6eyJtYXhfdHRsIjoyNTkyMDAwfX1dfQ==";
            var jsonDecoded = Base64Decode(encoded);

            var value = JsonConvert.DeserializeObject<TenantConfiguration>(jsonDecoded);
            value.Should().NotBeNull();
            value.Authority.Should().Be("https://apim-organics.azure-api.net/oauth2");


        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        [Fact]
        public void Test1()
        {
            var guidUrlShortener = new GuidUrlShortenerAlgorithm();
            var keys = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                var key = guidUrlShortener.GenerateUniqueId();
                keys.Add(key);
            }
            var d = keys[0];

        }
        [Fact]
        public void Test2()
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                var key = GuidUrlShortenerAlgorithm.ToBase62(i);
                keys.Add(key);
            }
            var d = keys[0];


        }
    }
}
